namespace Rhythm.Extensions.Mapping.Rules {
	using ExtensionMethods;
	using System;
	using System.Reflection;
	using Umbraco.Core.Models;
	public class PropertyMappingRule<T> : IMappingRule {
		private readonly string _propertyAlias;
		private readonly string _propertyName;

		public PropertyMappingRule(string propertyName, string propertyAlias) {
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

			var srcValue = content.LocalizedPropertyValue<T>(_propertyAlias);

			destProperty.SetValue(model, srcValue);
		}
	}
}