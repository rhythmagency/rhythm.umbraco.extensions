namespace Rhythm.Extensions.Mapping {
	using System;
	public interface IMappingRule {
		void Execute(MappingSession session, MappingOptions options, object model, Type type, object source);
	}
}