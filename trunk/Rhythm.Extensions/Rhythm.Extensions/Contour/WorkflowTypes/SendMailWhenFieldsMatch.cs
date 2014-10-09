using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using UmbracoLibrary = global::umbraco.library;
namespace Rhythm.Extensions.Contour.WorkflowTypes {

	/// <summary>
	/// Sends an email when the fields configured on the workflow match the values
	/// submitted to Contour.
	/// </summary>
	public class SendMailWhenFieldsMatch : WorkflowType {

		#region Properties

		/// <summary>
		/// The sender email address.
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Sender Address", description = "Enter sender email address.", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string SenderAddress { get; set; }

		/// <summary>
		/// The recipient email address.
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Recipient Address", description = "Enter recipient email address.", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string RecipientAddress { get; set; }

		/// <summary>
		/// The email subject line.
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Subject", description = "Enter email subject.", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string Subject { get; set; }

		/// <summary>
		/// The email message.
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Message", description = "Enter email message.", control = "Umbraco.Forms.Core.FieldSetting.TextArea")]
		public string Message { get; set; }

		/// <summary>
		/// Match all fields (or any field)?
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Match All", description = "Match all field values (otherwise,<br /> any matched value will send the email)?", control = "Umbraco.Forms.Core.FieldSetting.Checkbox")]
		public string MatchAll { get; set; }

		/// <summary>
		/// The field values to match.
		/// </summary>
		[global::Umbraco.Forms.Core.Attributes.Setting("Field Values", description = "Enter field values.", control = "Umbraco.Forms.Core.FieldSetting.FieldMapper")]
		public string FieldValues { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SendMailWhenFieldsMatch() {
			Id = new Guid("11E0243E-D94A-438E-8DF3-C2DE676B980F");
			Name = "Send Email When Field Matches Value";
			Description = "Sends an email when the specified fields match the specified values.";
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Executes the workflow.
		/// </summary>
		/// <param name="record">The record to execute the workflow on.</param>
		/// <returns>The execution status.</returns>
		public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e) {

			// Variables.
			var result = WorkflowExecutionStatus.Completed;

			// Catch exceptions.
			try {

				// Variables.
				var matchAll = GetMatchAll();
				var values = ParseFieldValues(FieldValues);
				var matchedAny = false;
				var matchedAll = true;

				// Check each field configured in workflow.
				foreach (var value in values) {

					// Find field matching configured field.
					var fields = record.RecordFields.Where(x => x.Value.Field.Id == value.Item1);
					if (fields.Any()) {

						// Variables.
						var pair = fields.First();
						var field = pair.Value.Field;
						var fieldField = pair.Value.Field;
						var prevalue = pair.Value.ValuesAsString();
						field.PreValueSource.Type.LoadSettings(field.PreValueSource);
						var strValue = field.PreValueSource.Type.GetPreValues(field)
							.Where(x => (x.Id ?? string.Empty).ToString() == prevalue)
							.Select(x => x.Value).FirstOrDefault();
						strValue = strValue ?? prevalue;

						// Did the field value match the configured value?
						if (strValue != null && value.Item2.InvariantEquals(strValue)) {
							matchedAny = true;
						} else {
							matchedAll = false;
						}

					} else {

						// Found a field that didn't match.
						matchedAll = false;

					}

				}
				
				// Were there matches?
				if (matchedAll || !matchAll && matchedAny) {

					// Variables.
					var lineFormat = @"{0}: ""{1}""";
					var lines = new List<string>();
					if (!string.IsNullOrWhiteSpace(Message)) {
						lines.Add(Message);
					}
					foreach (var field in record.RecordFields) {
						var caption = field.Value.Field.Caption;
						var value = field.Value.ValuesAsString();
						lines.Add(string.Format(lineFormat, caption, value));
					}
					var body = string.Join(Environment.NewLine, lines.ToArray());

					// Send email.
					UmbracoLibrary.SendMail(SenderAddress, RecipientAddress, Subject, body, false);

				}

			}
			catch (Exception ex) {

				// Log error.
				var message = "Error while executing SendMailWhenFieldsMatch workflow.";
				LogHelper.Error<SendMailWhenFieldsMatch>(message, ex);
				result = WorkflowExecutionStatus.Failed;

			}

			// Return execution result.
			return result;

		}

		/// <summary>
		/// Validate workflow settings.
		/// </summary>
		/// <returns>A list of errors, if any; otherwise, an empty list.</returns>
		public override List<Exception> ValidateSettings() {
			var errors = new List<Exception>();
			if (string.IsNullOrWhiteSpace(FieldValues) || ParseFieldValues(FieldValues).Count == 0) {
				errors.Add(new Exception("You must specify at least one field value."));
			}
			if (string.IsNullOrWhiteSpace(SenderAddress)) {
				errors.Add(new Exception("You must specify a sender address."));
			}
			if (string.IsNullOrWhiteSpace(RecipientAddress)) {
				errors.Add(new Exception("You must specify a recipient address."));
			}
			if (string.IsNullOrWhiteSpace(Subject)) {
				errors.Add(new Exception("You must specify a subject."));
			}
			return errors;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Get MatchAll as a boolean.
		/// </summary>
		/// <returns>The boolean version of MatchAll.</returns>
		private bool GetMatchAll() {
			return "True".InvariantEquals(MatchAll);
		}

		/// <summary>
		/// Parse the field values specified in the workflow.
		/// </summary>
		/// <param name="fieldValues">The serialized field values.</param>
		/// <returns>The parsed field values.</returns>
		private static List<Tuple<Guid, string>> ParseFieldValues(string fieldValues) {
			var values = new List<Tuple<Guid, string>>();
			var semicolon = ";".ToCharArray();
			var comma = ",".ToCharArray();
			var removeEmpties = StringSplitOptions.RemoveEmptyEntries;
			if (!string.IsNullOrWhiteSpace(fieldValues)) {
				var parts = fieldValues.Split(semicolon, removeEmpties)
					.Select(x => x.TrimEnd(comma)).ToList();
				foreach (var part in parts) {
					var pos = part.LastIndexOf(",");
					if (pos > 0) {
						var value = part.Substring(0, pos);
						var strGuid = part.Substring(pos + 1);
						var guid = Guid.Empty;
						if (Guid.TryParse(strGuid, out guid)) {
							values.Add(new Tuple<Guid,string>(guid, value));
						}
					}
				}
			}
			return values;
		}

		#endregion

	}

}