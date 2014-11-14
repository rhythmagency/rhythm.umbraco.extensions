namespace Rhythm.Extensions.Mapping {
	using Archetype.Models;
	public partial class MappingSession : IMappingSession {
		public ArchetypeMappingExecutor<T> Map<T>(ArchetypeModel model, string alias) where T : class {
			return new ArchetypeMappingExecutor<T>(this, model, alias);
		}
	}
}