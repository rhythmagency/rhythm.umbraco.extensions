namespace Rhythm.Extensions.Mapping {
	using System.Collections.Generic;
	using Umbraco.Core.Models;
	public interface IMappingSession {
		MappingExecutor<T> Map<T>(IEnumerable<IPublishedContent> contents) where T : class;
		MappingExecutor<T> Map<T>(IPublishedContent content) where T : class;
		MappingExecutor<T> Map<T>(int id) where T : class;
		MappingExecutor<T> Map<T>() where T : class;
		bool Contains(string key);
		T Get<T>(string key) where T : class;
		void Add(string key, object model);
		void Remove(string key);
	}
}