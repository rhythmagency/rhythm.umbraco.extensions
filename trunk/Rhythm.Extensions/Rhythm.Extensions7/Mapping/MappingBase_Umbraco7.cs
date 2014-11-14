namespace Rhythm.Extensions.Mapping {
	using Rules;
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	public abstract partial class MappingBase<T> : IMapping where T : class {
		public void Archetype<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			_rules.Add(member.Name, new ArchetypeMappingRule<TModel>(member.Name, alias));
		}

		public void ArchetypeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			_rules.Add(member.Name, new ArchetypeCollectionMappingRule<TModel>(member.Name, alias));
		}
	}
}