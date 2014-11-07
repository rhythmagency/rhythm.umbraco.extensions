using System;
using System.Reflection;
using Umbraco.Core.Models;

namespace Rhythm.Extensions.Mapping.Rules
{
	public class ComponentMappingRule<T> : IMappingRule where T : class, new()
	{
		private readonly string _propertyName;
		private readonly ComponentMapping<T> _componentMapping;

		public ComponentMappingRule(string propertyName, ComponentMapping<T> componentMapping)
		{
			_propertyName = propertyName;
			_componentMapping = componentMapping;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model, Type type, object source)
		{
			var content = source as IPublishedContent;

			if (content == null)
			{
				throw new Exception("Expected source type IPublishedContent");
			}

			var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

			var componentModel = new T();

			var rules = _componentMapping.GetRules();

			foreach (var rule in rules)
			{
				rule.Value.Execute(session, options, componentModel, typeof(T), content);
			}

			destProperty.SetValue(model, componentModel);
		}
	}
}