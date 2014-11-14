namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Reflection;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	using Helpers;
	public class UmbracoHelperMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly Func<UmbracoHelper, IPublishedContent> _filter;
		private readonly string _propertyName;

		public UmbracoHelperMappingRule(string propertyName, Func<UmbracoHelper, IPublishedContent> filter) {
			_propertyName = propertyName;
			_filter = filter;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var helper = ContentHelper.GetHelper();
			var relatedContent = session.Map<TModel>(_filter(helper)).WithOptions(options).Single();

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			destProperty.SetValue(model, relatedContent);
		}
	}
}