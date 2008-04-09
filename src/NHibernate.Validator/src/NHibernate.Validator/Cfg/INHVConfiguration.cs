using System.Collections.Generic;

namespace NHibernate.Validator.Cfg
{
	/// <summary>
	/// Contract for configuration sources.
	/// </summary>
	public interface INHVConfiguration
	{
		/// <summary>
		/// Configured properties
		/// </summary>
		IDictionary<string, string> Properties { get;}

		/// <summary>
		/// Configured Mappings
		/// </summary>
		IList<MappingConfiguration> Mappings { get;}
	}
}