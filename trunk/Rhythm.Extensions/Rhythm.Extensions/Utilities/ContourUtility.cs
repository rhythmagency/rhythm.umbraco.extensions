namespace Rhythm.Extensions.Utilities
{

	// Namespaces.
	using Rhythm.Extensions.Types;
	using System;
	using System.Collections.Generic;
	using Umbraco.Core;
	using Umbraco.Forms.Core.Services;
	using Umbraco.Forms.Data.Storage;
	using ValueLabelStrings = Types.ValueLabelPair<string, string>;
	using KeyValueStrings = System.Collections.Generic.KeyValuePair<string, string>;

	/// <summary>
	/// Utility to help with Contour.
	/// </summary>
	public class ContourUtility
	{

		#region Properties

		private static Dictionary<KeyValueStrings, InstanceCache<List<ValueLabelStrings>>> FieldValues { get; set; }
		private static object FieldValuesLock { get; set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ContourUtility()
		{
			FieldValues = new Dictionary<KeyValueStrings, InstanceCache<List<ValueLabelStrings>>>();
			FieldValuesLock = new object();
		}

		#endregion


		#region Methods

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

		#endregion

	}

}