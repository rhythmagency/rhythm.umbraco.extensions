using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rhythm.Extensions.DataTypes.FontAwesomeIconPicker {
	/// <summary>
	/// DropDownList control that lists all of the FontAwesome icons.
	/// </summary>
	public class DataEditor : DropDownList {
		public virtual bool TreatAsRichTextEditor { get { return false; } }
		public bool ShowLabel { get { return true; } }
		public Control Editor { get { return this; } }

		private const string FONT_AWESOME_ICON_PICKER_REQUIRE_KEY = "FontAwesomeIconPickerRequire";
		private const string FONT_AWESOME_ICON_PICKER_CSS_KEY = "FontAwesomeIconPickerCSS";
		private const string FONT_AWESOME_ICON_PICKER_SELECT_CSS_KEY = "FontAwesomeIconPickerSelectCSS";
		private const string FONT_AWESOME_ICON_PICKER_APP_CSS_KEY = "FontAwesomeIconPickerAppCSS";

		protected override void OnInit(EventArgs e) {
			this.CssClass = "umbEditorFontAwesomeIconPicker";

			if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(FONT_AWESOME_ICON_PICKER_REQUIRE_KEY)) {
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), FONT_AWESOME_ICON_PICKER_REQUIRE_KEY, "<script src='/App_Plugins/FontAwesomeIconPicker/js/app.min.js'></script>", false);
			}

			if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(FONT_AWESOME_ICON_PICKER_CSS_KEY)) {
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), FONT_AWESOME_ICON_PICKER_CSS_KEY, "<link rel='stylesheet' href='/App_Plugins/FontAwesomeIconPicker/css/font-awesome.css' />", false);
			}

			if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(FONT_AWESOME_ICON_PICKER_SELECT_CSS_KEY)) {
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), FONT_AWESOME_ICON_PICKER_SELECT_CSS_KEY, "<link rel='stylesheet' href='/App_Plugins/FontAwesomeIconPicker/js/node_modules/select2/select2.css' />", false);
			}

			if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(FONT_AWESOME_ICON_PICKER_APP_CSS_KEY)) {
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), FONT_AWESOME_ICON_PICKER_APP_CSS_KEY, "<link rel='stylesheet' href='/App_Plugins/FontAwesomeIconPicker/css/app.css' />", false);
			}

			base.OnInit(e);
		}
	}
}