namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Reflection;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public partial class NodeMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly string _propertyAlias;
		private readonly string _propertyName;
		private bool _isMedia;

		public NodeMappingRule(string propertyName, string propertyAlias) {
			_propertyName = propertyName;
			_propertyAlias = propertyAlias;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model, Type type, object source) {
			var content = source as IPublishedContent;

			if (content == null) {
				throw new Exception("Expected source type IPublishedContent");
			}

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var srcProperty = content.GetProperty(_propertyAlias);

			if (!PropertyHasValue(srcProperty)) {
				return;
			}

			var srcValue = srcProperty.Value;

			var helper = new UmbracoHelper(UmbracoContext.Current);

			var node = _isMedia ? helper.TypedMedia(srcValue) : helper.TypedContent(srcValue);

			var mappedNode = session.Map<TModel>(node).WithOptions(options).Single();

			destProperty.SetValue(model, mappedNode);
		}

		public void AsMedia() {
			_isMedia = true;
		}
	}
}