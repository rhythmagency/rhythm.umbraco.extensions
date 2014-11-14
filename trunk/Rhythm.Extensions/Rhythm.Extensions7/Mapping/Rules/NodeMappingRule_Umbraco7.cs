namespace Rhythm.Extensions.Mapping.Rules {
	using Umbraco.Core.Models;
	public partial class NodeMappingRule<TModel> : IMappingRule where TModel : class {
		private bool PropertyHasValue(IPublishedProperty property) {
			return property != null && property.HasValue;
		}
	}
}