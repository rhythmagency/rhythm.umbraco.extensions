namespace Rhythm.Extensions.Mapping.Rules {
	using ExtensionMethods;
	using Helpers;
	using System;
	using System.Reflection;
	using Umbraco.Core.Models;
	public class NodeMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly string _propertyAlias;
		private readonly string _propertyName;
		private bool _isMedia;

		public NodeMappingRule(string propertyName, string propertyAlias) {
			_propertyName = propertyName;
			_propertyAlias = propertyAlias;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var content = source as IPublishedContent;

			if (content == null) {
				throw new Exception("Expected source type IPublishedContent");
			}

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var srcValue = content.LocalizedPropertyValue<object>(_propertyAlias);

			if (srcValue == null || string.IsNullOrWhiteSpace(srcValue.ToString())) {
				return;
			}

			var helper = ContentHelper.GetHelper();

			var node = _isMedia ? helper.TypedMedia(srcValue) : helper.TypedContent(srcValue);

			var mappedNode = session.Map<TModel>(node).WithOptions(options).Single();

			destProperty.SetValue(model, mappedNode);
		}

		public void AsMedia() {
			_isMedia = true;
		}
	}
}