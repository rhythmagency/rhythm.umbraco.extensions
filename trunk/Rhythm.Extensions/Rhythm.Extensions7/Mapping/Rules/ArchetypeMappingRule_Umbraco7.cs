namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Linq;
	using System.Reflection;
	using Archetype.Models;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public partial class ArchetypeMappingRule<T> : IMappingRule where T : class {
		private readonly string _propertyName;
		private readonly string _propertyAlias;
		private readonly Type _type;

		public ArchetypeMappingRule(string propertyName, string alias) {
			_propertyName = propertyName;
			_propertyAlias = alias;
			_type = typeof(T);
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var dataTypeService = UmbracoContext.Current.Application.Services.DataTypeService;
			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			ArchetypeModel srcValue;
			IDataTypeDefinition dataTypeDefinition;

			if (source is ArchetypeFieldsetModel) {
				var fieldSet = source as ArchetypeFieldsetModel;
				var property = fieldSet.Properties.FirstOrDefault(x => x.Alias == _propertyAlias);

				srcValue = fieldSet.GetValue<ArchetypeModel>(_propertyAlias);
				dataTypeDefinition = dataTypeService.GetDataTypeDefinitionById(property.DataTypeId);
			} else {
				var content = source as IPublishedContent;
				var srcProperty = content.GetProperty(_propertyAlias);

				if (srcProperty == null) {
					return;
				}

				var propertyType = content.ContentType.GetPropertyType(_propertyAlias);

				dataTypeDefinition = dataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeId);

				srcValue = srcProperty.Value as ArchetypeModel;
			}

			if (srcValue == null) {
				return;
			}

			if (!srcValue.Any()) {
				return;
			}

			var archetypeModel = session.Map<T>(srcValue, dataTypeDefinition.Name).Single();

			destProperty.SetValue(model, archetypeModel);
		}
	}
}