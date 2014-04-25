namespace Rhythm.Extensions.Binding {

	// Namespaces.
	using Attributes;
	using System;
	using System.Web.Mvc;

	/// <summary>
	/// Adds some features to the default model binder.
	/// </summary>
	/// <remarks>
	/// This model binder should be set to the default on application start.
	/// Features:
	///		Bind model property to IP address when decorated with IpAddressBinderAttribute.
	///	</remarks>
	public class RhythmModelBinder : DefaultModelBinder {
		protected override void BindProperty(System.Web.Mvc.ControllerContext controllerContext,
			ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {
			
			// Workaround for MVC limitation/bug.
			// See: http://blog.baltrinic.com/software-development/dotnet/better-array-model-binding-in-asp-net-mvc
			// Essentially, any non-null array property on a model will throw an exception
			// when using the default model binder. This workaround forces empty arrays to be null.
			if (propertyDescriptor.PropertyType.BaseType == typeof(Array)) {
				var propertyArray = propertyDescriptor.GetValue(bindingContext.Model);
				if (propertyArray is Array && (propertyArray as Array).Length == 0)
				{
					propertyDescriptor.SetValue(bindingContext.Model, null);
				}
			}

			// Default binding.
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);

			// Set IP address.
			if (propertyDescriptor.Attributes.Contains(new IpAddressBinderAttribute())) {
				propertyDescriptor.SetValue(bindingContext.Model, controllerContext.HttpContext.Request.UserHostAddress);
			}

		}
	}

}