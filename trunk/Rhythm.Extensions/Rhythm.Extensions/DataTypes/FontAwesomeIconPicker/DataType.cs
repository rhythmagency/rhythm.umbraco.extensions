using System;
using Rhythm.Extensions.Utilities;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace Rhythm.Extensions.DataTypes.FontAwesomeIconPicker {
	/// <summary>
	/// FontAwesome Icon Picker data type.
	/// </summary>
	public sealed class DataType : AbstractDataEditor {
		private readonly DataEditor editor = new DataEditor();
		private IData data;

		public DataType() {
			this.RenderControl = this.editor;
			this.editor.Init += this.editor_Init;
			this.DataEditorControl.OnSave += DataEditorControl_OnSave;
		}

		public override Guid Id {
			get { return new Guid("6C7CEF8B-992B-4A8B-BDAD-243FE431E4AF"); }
		}

		public override string DataTypeName {
			get { return "FontAwesome Icon Picker"; }
		}

		public override IData Data {
			get { return this.data ?? (this.data = new DefaultData(this)); }
		}

		private void DataEditorControl_OnSave(EventArgs e) {
			this.Data.Value = this.editor.Text;
		}

		private void editor_Init(object sender, EventArgs e) {
			var parser = new FontAwesomeCssParser("/App_Plugins/FontAwesomeIconPicker/css/font-awesome.css");
			
			this.editor.Items.Clear();

			foreach (var className in parser.GetClassNames()) {
				this.editor.Items.Add(String.Format("icon-{0}", className));
			}
			
			if (this.Data.Value != null) {
				this.editor.SelectedValue = this.Data.Value.ToString();
			}
		}
	}
}
