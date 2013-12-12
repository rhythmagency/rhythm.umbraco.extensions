using System.Collections.Generic;
using System.Dynamic;

namespace Rhythm.Extensions.ExtensionMethods {
    public static class ExpandoObjectExtensionMethods {

        /// <summary>
        /// Checks if a property by the specified name exists on the specified object.
        /// </summary>
        /// <param name="item">The ExpandoObject.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>True, if the property exists; otherwise, false.</returns>
        public static bool HasProperty(this ExpandoObject item, string propertyName) {
            if (item != null) {
                if ((item as IDictionary<string, object>).ContainsKey(propertyName)) {
                    return true;
                }
            }
            return false;
        }

    }
}