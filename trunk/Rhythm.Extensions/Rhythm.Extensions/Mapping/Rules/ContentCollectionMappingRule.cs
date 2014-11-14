namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Umbraco.Core.Models;
	public class ContentCollectionMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly string _propertyName;
		private bool _isLazy;
		private readonly Func<IPublishedContent, IEnumerable<IPublishedContent>> _filter;

		public ContentCollectionMappingRule(string propertyName, Func<IPublishedContent,
			IEnumerable<IPublishedContent>> filter) {
			_propertyName = propertyName;
			_filter = filter;
			_isLazy = true;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			var content = source as IPublishedContent;

			if (content == null) {
				throw new Exception("Expected source type IPublishedContent");
			}

			if (!_isLazy || (options.IncludedProperties.ContainsKey(type) &&
				options.IncludedProperties[type].Contains(_propertyName))) {
				var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
					BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

				// Set to empty list. This will prevent loading the list
				// again if the nested session.Map<> contains a circular reference.
				destProperty.SetValue(model, new List<TModel>());

				var filtered = _filter(content);

				var collection = session.Map<TModel>(filtered).WithOptions(options).List();

				destProperty.SetValue(model, collection);
			}
		}

		public void Eager() {
			_isLazy = false;
		}
	}
}