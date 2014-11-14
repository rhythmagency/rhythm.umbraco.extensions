namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	using Umbraco.Core.Models;
	using Umbraco.Web;
	public interface IContentMapping : IMapping {
		Func<UmbracoHelper, IEnumerable<IPublishedContent>> ContentSource { get; }
	}
}