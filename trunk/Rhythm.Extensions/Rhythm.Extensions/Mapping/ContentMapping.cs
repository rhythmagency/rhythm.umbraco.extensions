namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public abstract class ContentMapping<T> : MappingBase<T>, IContentMapping where T : class {
		public Func<UmbracoHelper, IEnumerable<IPublishedContent>> ContentSource { get; private set; }

		protected void MapFrom(Func<UmbracoHelper, IEnumerable<IPublishedContent>> contentSource) {
			ContentSource = contentSource;
		}

		protected void Alias(string alias) {
			UmbracoMapper.RegisterType(alias, typeof(T));
		}
	}
}