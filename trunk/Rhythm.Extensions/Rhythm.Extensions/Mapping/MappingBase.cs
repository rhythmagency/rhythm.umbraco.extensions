namespace Rhythm.Extensions.Mapping {
	using Rules;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public abstract partial class MappingBase<T> : IMapping where T : class {
		public Type Type { get; private set; }
		private readonly IDictionary<string, IMappingRule> _rules;

		protected MappingBase() {
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

		public void Property<TModel>(Expression<Func<T, TModel>> property, string alias) {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new PropertyMappingRule<TModel>(member.Name, alias);

			_rules.Add(member.Name, rule);
		}

		public void Property(Expression<Func<T, object>> name, Func<IPublishedContent, object> sourceProperty) {
			var member = name.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var mapping = new CustomPropertyMappingRule(member.Name, sourceProperty);

			_rules.Add(member.Name, mapping);
		}

		public NodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new NodeMappingRule<TModel>(member.Name, alias);

			_rules.Add(member.Name, rule);

			return rule;
		}

		public void Parent<TModel>(Expression<Func<T, TModel>> property) where TModel : class {
			Content(property, x => x.Parent);
		}

		public ContentCollectionMappingRule<TModel> Children<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string childAlias) where TModel : class {
			return Children(property, new[] { childAlias });
		}

		public ContentCollectionMappingRule<TModel> Children<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, params string[] childAliases) where TModel : class {
			return Content(property, x => x.Children.Where(c => childAliases.Contains(c.DocumentTypeAlias)));
		}

		public void Content<TModel>(Expression<Func<T, TModel>> property, Func<IPublishedContent, IPublishedContent> filter) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new ContentMappingRule<TModel>(member.Name, filter);

			_rules.Add(member.Name, rule);
		}

		public ContentCollectionMappingRule<TModel> Content<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, Func<IPublishedContent, IEnumerable<IPublishedContent>> filter) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new ContentCollectionMappingRule<TModel>(member.Name, filter);

			_rules.Add(member.Name, rule);

			return rule;
		}

		public void Content<TModel>(Expression<Func<T, TModel>> property, Func<UmbracoHelper, IPublishedContent> filter) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new UmbracoHelperMappingRule<TModel>(member.Name, filter);

			_rules.Add(member.Name, rule);
		}

		public UmbracoHelperCollectionMappingRule<TModel> Content<TModel>(Expression<Func<T, TModel>> property, Func<UmbracoHelper, IEnumerable<IPublishedContent>> filter) where TModel : class {
			var member = property.Body.ToMember();

			if (member == null) {
				throw new Exception("param [property] must be a member expression");
			}

			var rule = new UmbracoHelperCollectionMappingRule<TModel>(member.Name, filter);

			_rules.Add(member.Name, rule);

			return rule;
		}

		public void Component<TModel>(Expression<Func<T, TModel>> property, Action<ComponentMapping<TModel>> action) where TModel : class, new() {
			var member = property.Body.ToMember();
			var mapper = new ComponentMapping<TModel>();

			action(mapper);

			_rules.Add(member.Name, new ComponentMappingRule<TModel>(member.Name, mapper));
		}

		public void Component<TModel>(Expression<Func<T, TModel>> property) where TModel : class, new() {
			/*TODO: Can't call GetMapperForType() here because the mapper might not be registered yet*/
			var member = property.Body.ToMember();
			var mapping = UmbracoMapper.GetMappingForType(typeof(TModel)) as ComponentMapping<TModel>;

			if (mapping == null) {
				throw new Exception(string.Format("No component mapper is defined for type: {0}", typeof(TModel).FullName));
			}

			_rules.Add(member.Name, new ComponentMappingRule<TModel>(member.Name, mapping));
		}
	}
}