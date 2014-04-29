using global::Umbraco.Core.Logging;
using global::Umbraco.Forms.Core;
using global::Umbraco.Forms.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using ActionUtility = Rhythm.Extensions.Utilities.ActionUtility;
using NetUtility = Rhythm.Extensions.Utilities.NetUtility;
using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace Rhythm.Extensions.Contour.WorkflowTypes {

	/// <summary>
	/// Workflow type that subscribes the user to a newsletter using the Exact Target API.
	/// </summary>
	public class SubscribeToExactTargetNewsletter : WorkflowType {

		private const string ErrorPostSubmit = "Error submitting HTTP POST to Exact Target API.";
		private const string ExactTargetProfileMessage = "Exact Target API call took {0}ms.";
		private const string ExactTargetSubscribeUrl = "http://cl{0}.exct.net/subscribe.aspx";

		[global::Umbraco.Forms.Core.Attributes.Setting("Exact Target List ID", description = "Enter the list ID", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string ExactTargetListId { get; set; }

		[global::Umbraco.Forms.Core.Attributes.Setting("Exact Target Instance", description = "Enter the intance (e.g., s1, s2, etc.)", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string ExactTargetInstance { get; set; }

		[global::Umbraco.Forms.Core.Attributes.Setting("Exact Target Member ID", description = "Enter the member ID", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
		public string ExactTargetMemberId { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SubscribeToExactTargetNewsletter() {
			Id = new Guid("81FA16D8-3EE9-4C07-AC85-B4C587DBA9C9");
			Name = "Subscribe to Newsletter with Exact Target API";
			Description = "Subscribes the user's email address to the newsletter using the Exact Target API.";
		}

		/// <summary>
		/// Subscribes the user to the newsletter.
		/// </summary>
		public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e) {

			// Variables.
			string email = null;
			string instancePart = "s1".InvariantEquals(ExactTargetInstance)
				? null
				: "." + ExactTargetInstance;
			string url = string.Format(ExactTargetSubscribeUrl, instancePart);

			// Gather field values.
			foreach (var recordField in record.RecordFields.Values) {
				object recordValue = recordField.Values.FirstOrDefault();
				string strRecordValue = ((recordValue as string) ?? string.Empty).TrimEnd();
				switch ((recordField.Field.Caption ?? string.Empty).ToLower()) {
					case "email":
						email = strRecordValue;
						break;
					default:
						// Ignore the other fields.
						break;
				}
			}

			// Put data into key/value list.
			List<StringPair> formData = new List<StringPair>() {
				new StringPair("MID", ExactTargetMemberId),
				new StringPair("lid", ExactTargetListId),
				new StringPair("SubAction", "sub_add_update"),
				new StringPair("Email Address", email)
			};

			// Send form.
			ActionUtility.SafeThread(() => {
				var startTime = DateTime.Now;
				var result = NetUtility.GetPostResponse(url, formData);
				var deltaTime = DateTime.Now.Subtract(startTime).TotalMilliseconds.ToString();
				LogHelper.Info(typeof(SubscribeToExactTargetNewsletter),
					() => string.Format(ExactTargetProfileMessage, deltaTime));
			}, ex => {
				LogHelper.Error(typeof(SubscribeToExactTargetNewsletter), ErrorPostSubmit, ex);
			});

			// Indicate success.
			return WorkflowExecutionStatus.Completed;

		}

		/// <summary>
		/// Validate workflow settings.
		/// </summary>
		/// <returns>A list of exceptions detailing any validation errors.</returns>
		public override List<Exception> ValidateSettings() {
			var errors = new List<Exception>();
			if (string.IsNullOrEmpty(ExactTargetListId)) {
				errors.Add(new Exception("List ID is required."));
			}
			if (string.IsNullOrEmpty(ExactTargetInstance)) {
				errors.Add(new Exception("Instance is required."));
			}
			if (string.IsNullOrEmpty(ExactTargetMemberId)) {
				errors.Add(new Exception("Member ID is required."));
			}
			return errors;
		}

	}

}