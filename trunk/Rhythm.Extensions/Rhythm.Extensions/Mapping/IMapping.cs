namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	public interface IMapping {
		Type Type { get; }
		IDictionary<string, IMappingRule> GetRules();
		IMappingRule GetRuleByProperty(string propertyName);
	}
}