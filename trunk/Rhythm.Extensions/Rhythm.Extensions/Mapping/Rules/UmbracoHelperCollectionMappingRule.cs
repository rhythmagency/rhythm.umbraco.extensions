namespace Rhythm.Extensions.Mapping.Rules {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public class UmbracoHelperCollectionMappingRule<TModel> : IMappingRule where TModel : class {
		private readonly Func<UmbracoHelper, IEnumerable<IPublishedContent>> _filter;
		private readonly string _propertyName;
		private bool _isLazy;

		public UmbracoHelperCollectionMappingRule(string propertyName, Func<UmbracoHelper,
			IEnumerable<IPublishedContent>> filter) {
			_propertyName = propertyName;
			_filter = filter;
			_isLazy = true;
		}

		void IMappingRule.Execute(MappingSession session, MappingOptions options, object model,
			Type type, object source) {
			if (!_isLazy || (options.IncludedProperties.ContainsKey(type) &&
				options.IncludedProperties[type].Contains(_propertyName))) {
				var filtered = _filter(new UmbracoHelper(UmbracoContext.Current));

				var collection = session.Map<TModel>(filtered).WithOptions(options).List();

				var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly |
					BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

				destProperty.SetValue(model, collection);
			}
		}

		public void Eager() {
			_isLazy = false;
		}
	}
}