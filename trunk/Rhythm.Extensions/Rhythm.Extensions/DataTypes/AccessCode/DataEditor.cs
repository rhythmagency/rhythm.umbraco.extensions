using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rhythm.Extensions.DataTypes.AccessCode {
	/// <summary>
	/// Simple Label control that displays the Access Code.
	/// </summary>
	public class DataEditor : Label {
		public virtual bool TreatAsRichTextEditor { get { return false; } }
		public bool ShowLabel { get { return true; } }
		public Control Editor { get { return this; } }

		protected override void OnInit(EventArgs e) {
			this.CssClass = "umbEditorLabel";
			base.OnInit(e);
		}
	}
}