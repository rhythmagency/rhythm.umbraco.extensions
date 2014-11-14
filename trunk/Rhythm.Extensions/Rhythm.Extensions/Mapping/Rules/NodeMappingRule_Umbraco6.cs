namespace Rhythm.Extensions.Mapping.Rules {
	using Umbraco.Core;
	using Umbraco.Core.Models;
	public partial class NodeMappingRule<TModel> : IMappingRule where TModel : class {
		private bool PropertyHasValue(IPublishedContentProperty property) {
			return property != null && !property.HasValue();
		}
	}
}