using System;
using System.Collections.Generic;
using Archetype.Models;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Extensions.Mapping
{
	public partial class MappingSession : IMappingSession
	{
		public ArchetypeMappingExecutor<T> Map<T>(ArchetypeModel model, string alias) where T : class
		{
			return new ArchetypeMappingExecutor<T>(this, model, alias);
		}
	}
}