namespace Rhythm.Extensions.ExtensionMethods
{

    // Namespaces.
	using System.IO;
	using System.Web.Mvc;


    /// <summary>
	/// Extension methods for the Controller class.
	/// </summary>
    public static class ControllerExtensionMethods
    {

        #region Methods

		/// <summary>
		/// Renders a view to a string.
		/// </summary>
		/// <param name="controller">The current controller.</param>
		/// <param name="viewPath">The path to the view.</param>
		/// <param name="model">The view model.</param>
		/// <param name="viewData">The view data dictionary. Optional.</param>
		/// <returns>The rendered view, as a string.</returns>
		/// <remarks>
		/// Useful, for example, when returning a JSON object containing the rendered view.
		/// </remarks>
		public static string RenderRazorViewToString(this Controller controller, string viewPath,
            object model, ViewDataDictionary viewData = null)
		{
			var tempData = new TempDataDictionary();
			var newViewData = (viewData == null) ? new ViewDataDictionary() : new ViewDataDictionary(viewData);
			newViewData.Model = model;
			var context = controller.ControllerContext;
			using (var sw = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindPartialView(context, viewPath);
				var viewContext = new ViewContext(context, viewResult.View, newViewData, tempData, sw);
				viewResult.View.Render(viewContext, sw);
				viewResult.ViewEngine.ReleaseView(context, viewResult.View);
				string renderedView = sw.GetStringBuilder().ToString();
				return renderedView;
			}
		}

		#endregion

    }

}