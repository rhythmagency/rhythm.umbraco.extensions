namespace Rhythm.Extensions.Mapping {
	using Archetype.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	public partial class ArchetypeMappingExecutor<T> where T : class {
		private readonly MappingSession _session;
		private readonly ArchetypeModel _model;
		private readonly IList<T> _results;
		private readonly Type _type;
		private MappingOptions _options;
		private readonly string _dataType;

		public ArchetypeMappingExecutor(MappingSession session, ArchetypeModel model, string dataType) {
			_session = session;
			_model = model;
			_type = typeof(T);
			_results = new List<T>();
			_options = new MappingOptions();
			_dataType = dataType;
		}

		private void Execute() {
			foreach (var fieldset in _model) {
				var alias = string.Format("{0}.{1}", _dataType, fieldset.Alias);
				var currentType = UmbracoMapper.GetRegisteredType(alias, _type);
				var typeHierarchy = currentType.GetHierarchy();

				var archetypeModel = Activator.CreateInstance(currentType) as T;

				foreach (var type in typeHierarchy) {
					var mapping = UmbracoMapper.GetMappingForType(type);

					if (mapping == null) {
						continue;
					}

					var rules = mapping.GetRules();

					foreach (var rule in rules) {
						rule.Value.Execute(_session, _options, archetypeModel, type, fieldset);
					}
				}

				_results.Add(archetypeModel);
			}
		}

		public T Single() {
			return List().FirstOrDefault();
		}

		public IEnumerable<T> List() {
			Execute();

			return _results;
		}
	}
}