using System;
using Umbraco.Core.Models;

namespace Rhythm.Extensions.Mapping
{
	public interface IMappingRule
	{
		void Execute(MappingSession session, MappingOptions options, object model, Type type, object source);
	}
}