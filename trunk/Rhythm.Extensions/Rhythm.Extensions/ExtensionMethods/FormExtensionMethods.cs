using System;
using System.Linq;
using Umbraco.Forms.Core;
using Umbraco.Forms.Data.Storage;

namespace Rhythm.Extensions.ExtensionMethods {
	public static class FormExtensionMethods {
		public static Form GetFormFromName(this FormStorage formStorage, string name) {
			return formStorage.GetAllForms().FirstOrDefault(x => x.Name.Equals(name));
		}

		public static Guid GetFormIdFromName(this FormStorage formStorage, string name) {
			var form = formStorage.GetFormFromName(name);
			return form == null ? Guid.Empty : form.Id;
		}
	}
}