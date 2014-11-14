namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Reflection;
	using Umbraco.Core;
	using Umbraco.Core.Models;
	public class CustomPropertyMappingRule : IMappingRule {
		private readonly string _propertyName;
		private readonly Func<IPublishedContent, object> _sourceProperty;

		public CustomPropertyMappingRule(string propertyName, Func<IPublishedContent, object> sourceProperty) {
			_propertyName = propertyName;
			_sourceProperty = sourceProperty;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var content = source as IPublishedContent;

			if (content == null) {
				throw new Exception("Expected source type IPublishedContent");
			}

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var contentValue = _sourceProperty(content);

			if (destProperty.PropertyType.IsInstanceOfType(contentValue)) {
				destProperty.SetValue(model, contentValue);
			} else {
				var convert = contentValue.TryConvertTo(destProperty.PropertyType);

				if (convert.Success) {
					destProperty.SetValue(model, convert.Result);
				}
			}
		}
	}
}