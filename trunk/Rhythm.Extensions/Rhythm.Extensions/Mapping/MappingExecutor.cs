using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbraco.Core.Models;

namespace Rhythm.Extensions.Mapping
{
	public class MappingExecutor<T> where T : class
	{
		private readonly MappingSession _session;
		private readonly IEnumerable<IPublishedContent> _contents;
		private readonly IList<T> _results;
		private readonly Type _type;
		private MappingOptions _options;

		public MappingExecutor(MappingSession session, IEnumerable<IPublishedContent> contents)
		{
			_session = session;
			_contents = contents;
			_type = typeof(T);
			_results = new List<T>();
			_options = new MappingOptions();
		}

		private void Execute()
		{
			foreach (var content in _contents)
			{
				var currentType = UmbracoMapper.GetRegisteredType(content.DocumentTypeAlias, _type);
				var typeHierarchy = currentType.GetHierarchy();

				var cacheKey = string.Format("{0}|{1}", content.Id, currentType.FullName);

				if (_session.Contains(cacheKey))
				{
					var cachedModel = _session.Get<T>(cacheKey);

					foreach (var type in typeHierarchy)
					{
						if (!_options.IncludedProperties.ContainsKey(type))
						{
							continue;
						}

						foreach (var propertyName in _options.IncludedProperties[type])
						{
							var prop = type.GetProperty(propertyName);

							if (prop.GetValue(cachedModel) != null)
							{
								continue;
							}

							var mapping = UmbracoMapper.GetMappingForType(type);

							if (mapping == null)
							{
								continue;
							}

							var rule = mapping.GetRuleByProperty(propertyName);

							rule.Execute(_session, _options, cachedModel, type, content);
						}
					}

					_results.Add(cachedModel);

					continue;
				}

				var model = Activator.CreateInstance(currentType) as T;

				_session.Add(cacheKey, model);

				foreach (var type in typeHierarchy)
				{
					var mapping = UmbracoMapper.GetMappingForType(type);

					if (mapping == null)
					{
						continue;
					}

					foreach (var rule in mapping.GetRules())
					{
						rule.Value.Execute(_session, _options, model, type, content);
					}
				}

				_results.Add(model);
			}
		}

		public T Single()
		{
			return List().FirstOrDefault();
		}

		public IEnumerable<T> List()
		{
			Execute();

			return _results;
		}

		public MappingExecutor<T> Include(Expression<Func<T, object>> expression)
		{
			var member = expression.Body.ToMember();

			if (!_options.IncludedProperties.ContainsKey(member.ReflectedType))
			{
				_options.IncludedProperties.Add(member.ReflectedType, new List<string>());
			}

			_options.IncludedProperties[member.ReflectedType].Add(member.Name);

			return this;
		}

		public MappingExecutor<T> WithOptions(MappingOptions options)
		{
			_options = options;

			return this;
		}
	}
}