namespace Rhythm.Extensions.Utilities
{

	// Namespaces.
	using System.Collections.Generic;
	using Umbraco.Core;
	using Umbraco.Forms.Core.Services;
	using Umbraco.Forms.Data.Storage;
	using ValueLabelStrings = Types.ValueLabelPair<string, string>;

	/// <summary>
	/// Utility to help with Contour.
	/// </summary>
	public class ContourUtility {

		/// <summary>
		/// Gets the field values for the specified field on the specified form.
		/// </summary>
		/// <param name="formName">The name of the Contour form.</param>
		/// <param name="fieldCaption">The caption for the field.</param>
		/// <returns>
		/// The field values and labels.
		/// </returns>
		public static List<ValueLabelStrings> GetFieldValues(string formName, string fieldCaption) {
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
		}

	}

}