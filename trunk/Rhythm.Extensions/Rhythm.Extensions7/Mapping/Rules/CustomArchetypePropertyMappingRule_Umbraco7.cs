namespace Rhythm.Extensions.Mapping.Rules {
	using Archetype.Models;
	using System;
	using System.Reflection;
	using Umbraco.Core;
	public partial class CustomArchetypePropertyMappingRule<T> : IMappingRule {
		private readonly string _propertyName;
		private readonly Func<ArchetypeFieldsetModel, object> _sourceProperty;

		public CustomArchetypePropertyMappingRule(string propertyName,
			Func<ArchetypeFieldsetModel, object> sourceProperty) {
			_propertyName = propertyName;
			_sourceProperty = sourceProperty;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var fieldset = source as ArchetypeFieldsetModel;

			if (fieldset == null) {
				return;
			}

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var fieldsetValue = _sourceProperty(fieldset);

			if (destProperty.PropertyType.IsInstanceOfType(fieldsetValue)) {
				destProperty.SetValue(model, fieldsetValue);
			} else {
				var convert = fieldsetValue.TryConvertTo(destProperty.PropertyType);
				if (convert.Success) {
					destProperty.SetValue(model, convert.Result);
				}
			}
		}
	}
}