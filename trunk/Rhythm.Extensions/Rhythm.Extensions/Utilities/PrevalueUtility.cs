using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using PreValue = umbraco.cms.businesslogic.datatype.PreValue;
using PreValues = umbraco.cms.businesslogic.datatype.PreValues;

namespace Rhythm.Extensions.Utilities {

	/// <summary>
	/// Utility to help work with Umbraco prevalues.
	/// </summary>
	public static class PrevalueUtility {

		#region Properties

		private static Dictionary<string, List<PreValue>> PrevaluesByType { get; set; }
		private static object PrevaluesByTypeLock { get; set; }
		private static Dictionary<string, List<string>> ValuesByType { get; set; }
		private static object ValuesByTypeLock { get; set; }
		private static List<Tuple<string, int>> AllPrevalueTypes { get; set; }
		private static object AllPrevalueTypesLock { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static PrevalueUtility()
		{
			PrevaluesByType = new Dictionary<string,List<PreValue>>();
			PrevaluesByTypeLock = new object();
			ValuesByType = new Dictionary<string,List<string>>();
			ValuesByTypeLock = new object();
			AllPrevalueTypes = new List<Tuple<string,int>>();
			AllPrevalueTypesLock = new object();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the prevalues for the Umbraco data type with the specified name.
		/// </summary>
		/// <param name="typeName">The name of the data type.</param>
		/// <returns>The prevalues.</returns>
		public static IEnumerable<PreValue> GetPrevaluesForType(string typeName) {
			var prevalues = null as List<PreValue>;
			lock (PrevaluesByTypeLock) {
				if (!PrevaluesByType.TryGetValue(typeName, out prevalues)) {
					prevalues = new List<PreValue>();
					foreach (var value in PreValues.GetPreValues(GetTypeId(typeName)).Values) {
						var casted = value as PreValue;
						if (casted != null) {
							prevalues.Add(value as PreValue);
						}
					}
					PrevaluesByType[typeName] = prevalues;
				}
			}
			return new List<PreValue>(prevalues);
		}

		/// <summary>
		/// Returns the prevalue values for the Umbraco data type with the specified  name.
		/// </summary>
		/// <param name="typeName">The name of the data type.</param>
		/// <returns>The values.</returns>
		public static IEnumerable<string> GetValuesForType(string typeName) {
			var values = null as List<string>;
			lock (ValuesByTypeLock) {
				if (!ValuesByType.TryGetValue(typeName, out values)) {
					var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
					values = new List<string>();
					values.AddRange(dataTypeService.GetPreValuesByDataTypeId(GetTypeId(typeName)));
					ValuesByType[typeName] = values;
				}
			}
			return new List<string>(values);
		}

		/// <summary>
		/// Gets the ID of the Umbraco data type with the specified name.
		/// </summary>
		/// <param name="typeName">The name of the data type.</param>
		/// <returns>The type ID.</returns>
		public static int GetTypeId(string typeName) {
			lock (AllPrevalueTypesLock) {
				if (!AllPrevalueTypes.Any()) {
					var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
					var allFullTypes = dataTypeService.GetAllDataTypeDefinitions();
					AllPrevalueTypes = allFullTypes.Select(x => new Tuple<string, int>(x.Name, x.Id)).ToList();
				}
			}
			return AllPrevalueTypes.FirstOrDefault(x => typeName.InvariantEquals(x.Item1)).Item2;
		}

		#endregion

	}

}