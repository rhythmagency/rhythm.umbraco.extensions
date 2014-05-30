using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Rhythm.Extensions.Utilities
{
	/// <summary>
	/// Utility to help with reflection operations.
	/// </summary>
	public class ReflectionUtility
	{
		/// <summary>
		/// Creates a struct with a field for each element in the specified collection of values.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="values">
		/// The array of values.
		/// The first property is the field name, and the second is the field value.
		/// </param>
		/// <param name="explicitType">
		/// If true, the type specified will be used for each field in the struct.
		/// If false, the type of each value will be used for the corresponding field in the struct.
		/// </param>
		/// <returns>
		/// An instance of the struct with the fields set to the specified values.
		/// </returns>
		public static object MapValuesToStructFields<T>(IEnumerable<Tuple<string, T>> values,
			bool explicitType = false)
		{
			// Variables.
			var name = "GeneratedStruct_" + Guid.NewGuid().ToString("N");
			var assemblyName = new AssemblyName(name);
			var access = AssemblyBuilderAccess.RunAndSave;
			var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, access);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("RuntimeGeneratedModule");
			var typeAttributes =
				TypeAttributes.Public |
				TypeAttributes.Sealed |
				TypeAttributes.SequentialLayout |
				TypeAttributes.Serializable;
			var typeBuilder = moduleBuilder.DefineType(name, typeAttributes, typeof (ValueType));

			// Avoid null reference exception.
			if (values == null)
			{
				values = new List<Tuple<string, T>>();
			}

			// Add fields to type.
			foreach (var value in values)
			{
				var fieldType = explicitType ? typeof (T) : value.Item2.GetType();
				typeBuilder.DefineField(value.Item1, fieldType, FieldAttributes.Public);
			}

			// Create type instance.
			var type = typeBuilder.CreateType();
			var instance = Activator.CreateInstance(type);
			foreach (var value in values)
			{
				type.GetField(value.Item1).SetValue(instance, value.Item2);
			}

			// Return instance.
			return instance;
		}
	}
}