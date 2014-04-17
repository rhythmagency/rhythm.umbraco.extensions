namespace Rhythm.Extensions.Binding {

	// Namespaces.
	using Attributes;
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

			// Default binding.
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);

			// Set IP address.
			if (propertyDescriptor.Attributes.Contains(new IpAddressBinderAttribute())) {
				propertyDescriptor.SetValue(bindingContext.Model, controllerContext.HttpContext.Request.UserHostAddress);
			}

		}
	}

}