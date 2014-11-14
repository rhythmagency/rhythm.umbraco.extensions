namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	public static class UmbracoMapper {
		private static readonly IDictionary<Type, IMapping> _mappings;
		private static readonly IDictionary<string, Type> _aliases;

		static UmbracoMapper() {
			_mappings = new Dictionary<Type, IMapping>();
			_aliases = new Dictionary<string, Type>();

			IncludeDefaultMappings();
		}

		private static void IncludeDefaultMappings() {
			_mappings.Add(typeof(PublishedContentBase), new PublishedContentBaseMap());
		}

		internal static void RegisterType(string alias, Type type) {
			_aliases.Add(alias, type);
		}

		internal static Type GetRegisteredType(string alias, Type defaultType) {
			var type = !_aliases.ContainsKey(alias) ? null : _aliases[alias];

			if (type == null) {
				return defaultType;
			}

			if (type == defaultType) {
				return defaultType;
			}

			if (!defaultType.IsAssignableFrom(type)) {
				return defaultType;
			}

			return type;
		}

		internal static IMapping GetMappingForType(Type type) {
			return !_mappings.ContainsKey(type) ? null : _mappings[type];
		}

		public static void AddMappingsFromAssemblyOf<T>() {
			AddMappingsFromAssembly(typeof(T).Assembly);
		}

		public static void AddMappingsFromAssembly(Assembly assembly) {
			var types = assembly.GetTypes().Where(t => typeof(IMapping).IsAssignableFrom(t)).ToList();

			foreach (var type in types) {
				var mapping = Activator.CreateInstance(type) as IMapping;

				if (mapping == null) {
					throw new Exception(string.Format("Type: {0} could not be converted to IMapping", type.FullName));
				}

				if (_mappings.ContainsKey(mapping.Type)) {
					throw new Exception(string.Format("A mapping for type: {0} already exists", mapping.Type));
				}

				_mappings.Add(mapping.Type, mapping);
			}
		}

		public static IMappingSession CreateSession() {
			return new MappingSession();
		}
	}
}