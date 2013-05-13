using System;
using System.Web;
using Rhythm.Extensions.Utilities;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace Rhythm.Extensions.DataTypes.AccessCode {
	/// <summary>
	/// Access Code data type. 
	/// </summary>
	public sealed class DataType : AbstractDataEditor {
		private readonly DataEditor editor = new DataEditor();
		private IData data;
		private PrevalueEditor prevalueEditor;
		private const int MIN_PASSWORD_LENGTH = 2;

		public DataType() {
			this.RenderControl = this.editor;
			this.editor.Init += this.editor_Init;
			this.DataEditorControl.OnSave += DataEditorControl_OnSave;
		}

		public override Guid Id {
			get { return new Guid("6DECB042-B4F6-4BF2-9CF7-2E56EB100726"); }
		}

		public override string DataTypeName {
			get { return "Access Code"; }
		}

		public override IDataPrevalue PrevalueEditor {
			get { return this.prevalueEditor ?? (this.prevalueEditor = new PrevalueEditor(this)); }
		}

		public override IData Data {
			get { return this.data ?? (this.data = new DefaultData(this)); }
		}

		private void DataEditorControl_OnSave(EventArgs e) {
			this.Data.Value = this.editor.Text;
		}

		/// <summary>
		/// If the data is null or has never been saved we'll generate a new password based on the configuration. If it has been saved before it is permanantly locked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void editor_Init(object sender, EventArgs e) {
			var configuration = ((PrevalueEditor)PrevalueEditor).Configuration;

			if (this.Data.Value != null) {
				this.editor.Text = this.Data.Value.ToString();
			} else {
				if (this.data.Value == null || String.IsNullOrWhiteSpace(this.data.Value.ToString())) {
					if (configuration.IncludeIdPrefix) {
						var id = HttpContext.Current.Request.QueryString["id"];
						var pw = PasswordGenerator.Generate(Math.Max(MIN_PASSWORD_LENGTH, configuration.Length - id.Length));
						this.data.Value = String.Format("{0}{1}", pw, id);
					} else {
						this.data.Value = PasswordGenerator.Generate(Math.Max(MIN_PASSWORD_LENGTH, configuration.Length));
					}
				}
			}
		}
	}
}
