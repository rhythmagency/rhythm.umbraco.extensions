namespace Rhythm.Extensions.Mapping.Rules {
	using Archetype.Models;
	using Helpers;
	using System;
	using System.Reflection;
	public class ArchetypeNodeMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly string _propertyAlias;
		private readonly string _propertyName;
		private bool _isMedia;

		public ArchetypeNodeMappingRule(string propertyName, string propertyAlias) {
			_propertyName = propertyName;
			_propertyAlias = propertyAlias;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var fieldset = source as ArchetypeFieldsetModel;

			if (fieldset == null) {
				return;
			}

			if (!fieldset.HasProperty(_propertyAlias) || !fieldset.HasValue(_propertyAlias)) {
				return;
			}

			var srcValue = fieldset.GetValue<string>(_propertyAlias);

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