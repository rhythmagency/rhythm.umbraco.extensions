using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;

namespace Rhythm.Extensions.DataTypes.AccessCode {
	/// <summary>
	/// Access Code Prevalue Editor. Has a few simple options for configuring the Access Code data type.
	/// </summary>
	public class PrevalueEditor : PlaceHolder, IDataPrevalue {
		#region IDataPrevalue Members

		private readonly BaseDataType datatype;
		private CheckBox checkBoxIncludeIdPrefix;
		private TextBox textBoxLength;

		public PrevalueEditor(BaseDataType dataType) {
			this.datatype = dataType;
			this.SetupChildControls();
		}

		/// <summary>
		/// Create the controls needed for entering configuration data.
		/// </summary>
		private void SetupChildControls() {
			this.checkBoxIncludeIdPrefix = new CheckBox() { ID = "checkBoxIncludeIdPrefix", CssClass = "umbEditorCheckBox" };
			this.textBoxLength = new TextBox() {ID = "textBoxLength", CssClass = "umbEditorTextBox"};

			// JavaScript to replace non-integer characters
			this.textBoxLength.Attributes["onkeyup"] = @"javascript:(function(el){ if(el.value.length > 0) { el.value = el.value.replace(/[^\d]+/g, ''); } })(this)";

			this.Controls.Add(this.checkBoxIncludeIdPrefix);
			this.Controls.Add(this.textBoxLength);
		}

		public Control Editor { get { return this; } }

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			if (Page.IsPostBack) return;

			// Prefill the configuration controls based on the configuration object.
			this.checkBoxIncludeIdPrefix.Checked = this.Configuration.IncludeIdPrefix;
			this.textBoxLength.Text = this.Configuration.Length.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Saves the current configuration to the database.
		/// </summary>
		public void Save() {
			this.datatype.DBType = (DBTypes)Enum.Parse(typeof(DBTypes), DBTypes.Ntext.ToString(), true);
			this.DeleteAllDataTypePreValues();
			this.AddDataTypePreValue("includeIdPrefix", this.checkBoxIncludeIdPrefix.Checked.ToString());
			this.AddDataTypePreValue("length", this.textBoxLength.Text);
		}

		protected override void Render(HtmlTextWriter writer) {
			writer.WriteLine("<table>");
			writer.Write("<tr><th>Include ID Prefix:</th><td>");
			this.checkBoxIncludeIdPrefix.RenderControl(writer);
			writer.Write("<tr><th>Access Code Length:</th><td>");
			this.textBoxLength.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("</table>");
		}

		/// <summary>
		/// Helper method that generates the datatypenodeid property.
		/// </summary>
		/// <returns>SQL Parameter containing the datatypenodeid property.</returns>
		private IParameter GetDataTypeDefinitionIdParam() {
			return SqlHelper.CreateParameter("@datatypenodeid", this.datatype.DataTypeDefinitionId);
		}

		/// <summary>
		/// Returns a prevalue for the current data type based on the alias supplied.
		/// </summary>
		/// <typeparam name="T">The type to return.</typeparam>
		/// <param name="alias">The alias of the prevalue.</param>
		/// <returns>Returns a prevalue for the current data type based on the alias supplied.</returns>
		private T GetDataTypePreValue<T>(string alias) {
			var dataTypeDefIdParam = this.GetDataTypeDefinitionIdParam();
			var aliasParam = SqlHelper.CreateParameter("@alias", alias);
			return SqlHelper.ExecuteScalar<T>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid and alias = @alias", dataTypeDefIdParam, aliasParam);
		}

		/// <summary>
		/// Adds a prevalue for the current data type.
		/// </summary>
		/// <param name="alias">The alias of the prevalue.</param>
		/// <param name="value">The value of the prevalue.</param>
		/// <returns>Returns the number of rows affected.</returns>
		private int AddDataTypePreValue(string alias, object value) {
			var dataTypeDefIdParam = this.GetDataTypeDefinitionIdParam();
			var valueParam = SqlHelper.CreateParameter("@value", value);
			var aliasParam = SqlHelper.CreateParameter("@alias", alias);
			return SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues ([datatypenodeid], [value], [sortorder], [alias]) values (@datatypenodeid, @value, 0, @alias)", dataTypeDefIdParam, valueParam, aliasParam);
		}

		/// <summary>
		/// Removes all of the prevalues in the database for this data type.
		/// </summary>
		/// <returns>Returns the number of rows affected.</returns>
		private int DeleteAllDataTypePreValues() {
			var dataTypeDefIdParam = this.GetDataTypeDefinitionIdParam();
			return SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", dataTypeDefIdParam);
		}

		/// <summary>
		/// Configuration object that contains whether to include the ID prefix and how long the access code should be (including the ID prefix).
		/// </summary>
		public Configuration Configuration {
			get {
				var configuration = new Configuration();

				var includeIdPrefix = this.GetDataTypePreValue<object>("includeIdPrefix");
				var length = this.GetDataTypePreValue<object>("length");

				if (includeIdPrefix != null) {
					bool.TryParse(includeIdPrefix.ToString(), out configuration.IncludeIdPrefix);	
				}

				if (length != null) {
					int.TryParse(length.ToString(), out configuration.Length);	
				}

				return configuration;
			}
		}

		#endregion

		public static ISqlHelper SqlHelper {
			get { return Application.SqlHelper; }
		}
	}
}