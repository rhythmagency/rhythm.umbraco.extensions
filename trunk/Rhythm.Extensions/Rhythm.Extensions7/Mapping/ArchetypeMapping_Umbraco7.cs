namespace Rhythm.Extensions.Mapping {
	using Archetype.Models;
	using Rules;
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	public class ArchetypeMapping<T> : IMapping where T : class {
		public Type Type { get; private set; }
		private readonly IDictionary<string, IMappingRule> _rules;

		public ArchetypeMapping() {
			Type = typeof(T);
			_rules = new Dictionary<string, IMappingRule>();
		}

		public IDictionary<string, IMappingRule> GetRules() {
			return _rules;
		}

		public IMappingRule GetRuleByProperty(string name) {
			if (!_rules.ContainsKey(name)) {
				throw new Exception(string.Format("No rule exists for property {0}", name));
			}

			return _rules[name];
		}

		protected void Alias(string dataTypeName, string propertyAlias) {
			UmbracoMapper.RegisterType(string.Format("{0}.{1}", dataTypeName, propertyAlias), typeof(T));
		}

		protected void Property<TModel>(Expression<Func<T, TModel>> property, string alias) {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new ArchetypePropertyMappingRule<TModel>(member.Name, alias);

			_rules.Add(member.Name, rule);
		}

		protected void Property<TModel>(Expression<Func<T, TModel>> property,
			Func<ArchetypeFieldsetModel, object> sourceProperty)
		{
			var member = property.Body.ToMember();

			if (member == null)
			{
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new CustomArchetypePropertyMappingRule<TModel>(member.Name, sourceProperty);

			_rules.Add(member.Name, rule);
		}

		protected ArchetypeNodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new ArchetypeNodeMappingRule<TModel>(member.Name, alias);

			_rules.Add(member.Name, rule);

			return rule;
		}

		protected void Archetype<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			_rules.Add(member.Name, new ArchetypeMappingRule<TModel>(member.Name, alias));
		}

		protected void ArchetypeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			_rules.Add(member.Name, new ArchetypeCollectionMappingRule<TModel>(member.Name, alias));
		}
	}
}