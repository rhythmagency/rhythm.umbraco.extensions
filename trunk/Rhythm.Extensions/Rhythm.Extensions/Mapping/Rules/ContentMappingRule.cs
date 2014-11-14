namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Reflection;
	using Umbraco.Core.Models;
	public class ContentMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly Func<IPublishedContent, IPublishedContent> _filter;
		private readonly string _propertyName;

		public ContentMappingRule(string propertyName, Func<IPublishedContent, IPublishedContent> filter) {
			_propertyName = propertyName;
			_filter = filter;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var content = source as IPublishedContent;

			if (content == null) {
				throw new Exception("Expected source type IPublishedContent");
			}

			var relatedContent = session.Map<TModel>(_filter(content)).WithOptions(options).Single();

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			destProperty.SetValue(model, relatedContent);
		}
	}
}