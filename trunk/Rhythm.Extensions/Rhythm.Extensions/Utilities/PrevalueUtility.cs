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

        /// <summary>
        /// Returns the prevalues for the Umbraco data type with the specified name.
        /// </summary>
        /// <param name="typeName">The name of the data type.</param>
        /// <returns>The prevalues.</returns>
        public static IEnumerable<PreValue> GetPrevaluesForType(string typeName) {
            var values = PreValues.GetPreValues(GetTypeId(typeName)).Values;
            foreach (var value in values) {
                yield return value as PreValue;
            }
        }

        /// <summary>
        /// Returns the prevalue values for the Umbraco data type with the specified  name.
        /// </summary>
        /// <param name="typeName">The name of the data type.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<string> GetValuesForType(string typeName) {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            return dataTypeService.GetPreValuesByDataTypeId(GetTypeId(typeName));
        }

        /// <summary>
        /// Gets the ID of the Umbraco data type with the specified name.
        /// </summary>
        /// <param name="typeName">The name of the data type.</param>
        /// <returns>The type ID.</returns>
        public static int GetTypeId(string typeName) {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            var allTypes = dataTypeService.GetAllDataTypeDefinitions();
            return allTypes.FirstOrDefault(x => typeName.InvariantEquals(x.Name)).Id;
        }

    }

}