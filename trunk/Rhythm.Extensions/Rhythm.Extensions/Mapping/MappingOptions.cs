namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	public class MappingOptions {
		public IDictionary<Type, IList<string>> IncludedProperties { get; private set; }

		public MappingOptions() {
			IncludedProperties = new Dictionary<Type, IList<string>>();
		}
	}
}