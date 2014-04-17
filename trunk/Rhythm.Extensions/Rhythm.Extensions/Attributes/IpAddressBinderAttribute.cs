namespace Rhythm.Extensions.Attributes {

	// Namespace.
	using System;

	/// <summary>
	/// This attribute will cause any property it decorates to be set to the request IP address.
	/// </summary>
	/// <remarks>
	/// This requires that the default binder be set to the RhythmModelBinder.
	/// </remarks>
	public class IpAddressBinderAttribute : Attribute {
	}

}