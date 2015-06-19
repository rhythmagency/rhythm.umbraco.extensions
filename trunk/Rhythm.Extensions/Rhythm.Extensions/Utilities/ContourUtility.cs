namespace Rhythm.Extensions.Utilities
{

	// Namespaces.
	using Rhythm.Extensions.Types;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Umbraco.Core;
	using Umbraco.Forms.Core;
	using Umbraco.Forms.Core.Services;
	using Umbraco.Forms.Data.Storage;
	using KeyValueStrings = System.Collections.Generic.KeyValuePair<string, string>;
	using ValueLabelStrings = Types.ValueLabelPair<string, string>;

	/// <summary>
	/// Utility to help with Contour.
	/// </summary>
	public class ContourUtility {

		#region Properties

		private static Dictionary<KeyValueStrings, InstanceCache<List<ValueLabelStrings>>> FieldValues { get; set; }
		private static object FieldValuesLock { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ContourUtility() {
			FieldValues = new Dictionary<KeyValueStrings, InstanceCache<List<ValueLabelStrings>>>();
			FieldValuesLock = new object();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the field values for the specified field on the specified form.
		/// </summary>
		/// <param name="formName">The name of the Contour form.</param>
		/// <param name="fieldCaption">The caption for the field.</param>
		/// <returns>
		/// The field values and labels.
		/// </returns>
		public static List<ValueLabelStrings> GetFieldValues(string formName, string fieldCaption) {

			// Validation.
			if (string.IsNullOrWhiteSpace(formName) || string.IsNullOrWhiteSpace(fieldCaption)) {
				return null;
			}

			// Get cache of values for specified form/field.
			var cache = null as InstanceCache<List<ValueLabelStrings>>;
			lock (FieldValuesLock) {
				var key = new KeyValueStrings(formName, fieldCaption);
				if (!FieldValues.TryGetValue(key, out cache)) {
					cache = new InstanceCache<List<ValueLabelStrings>>();
					FieldValues[key] = cache;
				}
			}

			// Get values from cache.
			return cache.Get(TimeSpan.FromHours(3), () => {
				var results = new List<ValueLabelStrings>();
				using (var formStorage = new FormStorage()) {
					var form = formStorage.GetForm(formName);
					using (var recordService = new RecordService(form)) {
						recordService.Open();
						foreach (var field in recordService.Form.AllFields) {
							if (fieldCaption.InvariantEquals(field.Caption)) {
								field.PreValueSource.Type.LoadSettings(field.PreValueSource);
								foreach (var item in field.PreValueSource.Type.GetPreValues(field)) {
									results.Add(new ValueLabelStrings(item.Id.ToString(), item.Value));
								}
							}
						}
					}
				}
				return results;
			});

		}

		/// <summary>
		/// Stores a form submission to Contour.
		/// </summary>
		/// <param name="formName">The name of the form.</param>
		/// <param name="fieldValues">The values to store to contour.</param>
		/// <param name="ipAddress">The user's IP address.</param>
		/// <param name="pageId">The ID of the page the user entered the values on.</param>
		/// <returns>True, if the data was stored successfully; otherwise, false.</returns>
		public static bool StoreRecord(string formName, Dictionary<string, List<object>> fieldValues,
			string ipAddress, int pageId) {

			// Variables.
			var success = true;

			// Ensure keys are lowercase.
			fieldValues = GetLowercaseKeys(fieldValues);

			// Add record.
			using (var formStorage = new FormStorage()) {
				var form = formStorage.GetForm(formName);
				var tempGuid = Guid.Empty;
				success = success && StoreRecord(form, fieldValues, ipAddress, pageId, out tempGuid);
			}

			// Succeeded?
			return success;

		}

		/// <summary>
		/// Stores a form submission to Contour.
		/// </summary>
		/// <param name="formGuid">The GUID of the form.</param>
		/// <param name="fieldValues">The values to store to contour.</param>
		/// <param name="ipAddress">The user's IP address.</param>
		/// <param name="pageId">The ID of the page the user entered the values on.</param>
		/// <returns>True, if the data was stored successfully; otherwise, false.</returns>
		public static bool StoreRecord(Guid formGuid, Dictionary<string, List<object>> fieldValues,
			string ipAddress, int pageId) {

			// Variables.
			var success = true;

			// Ensure keys are lowercase.
			fieldValues = GetLowercaseKeys(fieldValues);

			// Add record.
			using (var formStorage = new FormStorage()) {
				var form = formStorage.GetForm(formGuid);
				var tempGuid = Guid.Empty;
				success = success && StoreRecord(form, fieldValues, ipAddress, pageId, out tempGuid);
			}

			// Succeeded?
			return success;

		}

		/// <summary>
		/// Stores a form submission to Contour.
		/// </summary>
		/// <param name="formName">The name of the form.</param>
		/// <param name="fieldValues">The values to store to contour.</param>
		/// <param name="ipAddress">The user's IP address.</param>
		/// <param name="pageId">The ID of the page the user entered the values on.</param>
		/// <param name="recordId">Outputs the ID of the record stored to Contour.</param>
		/// <returns>True, if the data was stored successfully; otherwise, false.</returns>
		public static bool StoreRecord(string formName, Dictionary<string, List<object>> fieldValues,
			string ipAddress, int pageId, out Guid recordId)
		{

			// Variables.
			var success = true;
			recordId = Guid.Empty;

			// Ensure keys are lowercase.
			fieldValues = GetLowercaseKeys(fieldValues);

			// Add record.
			using (var formStorage = new FormStorage())
			{
				var form = formStorage.GetForm(formName);
				success = success && StoreRecord(form, fieldValues, ipAddress, pageId, out recordId);
			}

			// Succeeded?
			return success;

		}

		/// <summary>
		/// Stores a form submission to Contour.
		/// </summary>
		/// <param name="formGuid">The GUID of the form.</param>
		/// <param name="fieldValues">The values to store to contour.</param>
		/// <param name="ipAddress">The user's IP address.</param>
		/// <param name="pageId">The ID of the page the user entered the values on.</param>
		/// <param name="recordId">Outputs the ID of the record stored to Contour.</param>
		/// <returns>True, if the data was stored successfully; otherwise, false.</returns>
		public static bool StoreRecord(Guid formGuid, Dictionary<string, List<object>> fieldValues,
			string ipAddress, int pageId, out Guid recordId) {

			// Variables.
			var success = true;
			recordId = Guid.Empty;

			// Ensure keys are lowercase.
			fieldValues = GetLowercaseKeys(fieldValues);

			// Add record.
			using (var formStorage = new FormStorage())
			{
				var form = formStorage.GetForm(formGuid);
				success = success && StoreRecord(form, fieldValues, ipAddress, pageId, out recordId);
			}

			// Succeeded?
			return success;

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts a dictionary of values so each key is lowercase.
		/// </summary>
		/// <param name="fieldValues">The original dictionary.</param>
		/// <returns>The dictionary with lowercase keys.</returns>
		private static Dictionary<string,List<object>> GetLowercaseKeys(
			Dictionary<string, List<object>> fieldValues) {
			fieldValues = new Dictionary<string,List<object>>(fieldValues);
			var oldKeys = fieldValues.Keys;
			foreach (var key in oldKeys) {
				var lowerKey = key == null ? key : key.ToLower();
				if (key != lowerKey) {
					var value = fieldValues[key];
					fieldValues.Remove(key);
					fieldValues[lowerKey] = value;
				}
			}
			return fieldValues;
		}

		/// <summary>
		/// Stores a form record.
		/// </summary>
		/// <param name="form">The form to store a record to.</param>
		/// <param name="fieldValues">The field values to store.</param>
		/// <param name="ipAddress">The user's IP address.</param>
		/// <param name="pageId">The current page ID.</param>
		/// <param name="recordId">Outputs the ID of the record stored to Contour.</param>
		/// <returns>True, if the record was stored; otherwise, false.</returns>
		private static bool StoreRecord(Form form, Dictionary<string, List<object>> fieldValues,
			string ipAddress, int pageId, out Guid recordId) {
			using (var recordStorage = new RecordStorage())
			using (var recordService = new RecordService(form)) {

				// Open record service.
				recordService.Open();

				// Create record.
				var record = recordService.Record;
				record.IP = ipAddress;
				record.UmbracoPageId = pageId;
				record = recordStorage.InsertRecord(record, form);

				// Assign field values for record.
				foreach (var field in recordService.Form.AllFields) {

					// Check which field this is.
					var knownField = false;
					var values = default(List<object>);
					if (fieldValues.TryGetValue((field.Caption ?? string.Empty).ToLower(), out values)) {
						knownField = true;
					}

					// If the field is one of those that are known, store it.
					if (knownField && values != null && values.Any(x => x != null)) {

						// Create field.
						var key = Guid.NewGuid();
						var recordField = new RecordField {
							Field = field,
							Key = key,
							Record = record.Id,
							Values = values
						};

						// Store field.
						using (var recordFieldStorage = new RecordFieldStorage()) {
							recordFieldStorage.InsertRecordField(recordField);
						}

						// Add field to record.
						record.RecordFields.Add(key, recordField);

					}

				}

				// Submit / save record.
				recordService.Submit();
				var result = recordService.SaveFormToRecord();
				recordId = record.Id;
				return result;

			}
		}

		#endregion

	}

}