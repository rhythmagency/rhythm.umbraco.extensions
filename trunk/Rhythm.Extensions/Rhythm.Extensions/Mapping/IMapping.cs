using System;
using System.Collections.Generic;

namespace Rhythm.Extensions.Mapping
{
	public interface IMapping
	{
		Type Type { get; }
		IDictionary<string, IMappingRule> GetRules();
		IMappingRule GetRuleByProperty(string propertyName);
	}
}