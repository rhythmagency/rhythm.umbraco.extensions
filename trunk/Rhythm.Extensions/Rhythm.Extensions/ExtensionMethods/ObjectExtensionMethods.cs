using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Rhythm.Extensions.ExtensionMethods
{
    public static class ObjectExtensionMethods
    {

        /// <summary>
        /// Makes the specified object an expando object.
        /// </summary>
        /// <param name="source">The object to convert.</param>
        /// <returns>An ExpandoObject version of the supplied object.</returns>
        /// <remarks>
        /// This is useful to pass around anonymous objects.
        /// </remarks>
        public static ExpandoObject ToExpando(this object source)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(source.GetType()))
            {
                expando.Add(prop.Name, prop.GetValue(source));
            }
            return expando as ExpandoObject;
        }

    }
}