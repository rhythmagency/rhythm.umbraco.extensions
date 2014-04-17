namespace Rhythm.Extensions.Attributes {

	// Namespaces.
	using Enums;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Reflection;
	using Utilities;
	using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

	/// <summary>
	/// Validates a recaptcha solution.
	/// </summary>
	/// <remarks>
	/// This attribute should be set on the captcha solution property in the same model
	/// as a property marked with IpAddressBinderAttribute.
	/// </remarks>
	public class ValidateRecaptchaAttribute : ValidationAttribute {

		#region Constants

		private const string RECAPTCHA_VERIFY_URL = "http://www.google.com/recaptcha/api/verify";
		private const string DEFAULT_ERROR_MESSAGE = "Captcha solution did not pass validation.";

		#endregion

		#region Properties

		/// <summary>
		/// The name of the Captcha challenge field.
		/// </summary>
		private string ChallengeFieldName { get; set; }

		#endregion

		#region Constructors

		public ValidateRecaptchaAttribute(string challengeFieldName) {
			this.ErrorMessage = DEFAULT_ERROR_MESSAGE;
			this.ChallengeFieldName = challengeFieldName;
		}

		#endregion

		#region Validation Methods

		/// <summary>
		/// Check if the Captcha is valid.
		/// </summary>
		/// <param name="value">The Captcha solution.</param>
		/// <param name="validationContext">Validation context.</param>
		/// <returns>The validation result (success or error).</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

			// Variables.
			var ignoreCase = StringComparison.InvariantCultureIgnoreCase;
			var privateKey = ConfigUtility.GetString(ConfigKeys.RecaptchaPrivateKey);
			var strChallenge = GetChallenge(validationContext);
			var strSolution = value as string;
			var ipAddress = GetIpAddress(validationContext);

			// Check Google to see if Captcha solution is valid.
			string strResponse = NetUtility.GetPostResponse(RECAPTCHA_VERIFY_URL,
				new List<StringPair>() {
					new StringPair("privatekey", privateKey),
					new StringPair("remoteip", ipAddress),
					new StringPair("challenge", strChallenge),
					new StringPair("response", strSolution)
				});

			// Indicate success or failure.
			bool success = (strResponse ?? string.Empty).StartsWith("true", ignoreCase);
			return success
				? ValidationResult.Success
				: new ValidationResult(ErrorMessage);

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Get the Captcha challenge from the validation context.
		/// </summary>
		/// <param name="validationContext">The validation context.</param>
		/// <returns>The Captcha challenge.</returns>
		private string GetChallenge(ValidationContext validationContext) {
			var propInfo = validationContext.ObjectType.GetProperty(ChallengeFieldName);
			var fieldValue = propInfo.GetValue(validationContext.ObjectInstance);
			return fieldValue as string;
		}

		/// <summary>
		/// Gets the IP address from the validation context.
		/// </summary>
		/// <param name="validationContext">The validation context.</param>
		/// <returns>The IP address.</returns>
		private string GetIpAddress(ValidationContext validationContext) {

			// Variables.
			var ipAddress = string.Empty;

			// Look for the IP address in the model properties.
			var properties = validationContext.ObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var prop in properties) {
				foreach (var attribute in prop.CustomAttributes) {
					if (attribute.AttributeType == typeof(IpAddressBinderAttribute)) {
						ipAddress = prop.GetValue(validationContext.ObjectInstance) as string;
					}
				}
			}

			// Return the IP address.
			return ipAddress;

		}

		#endregion

	}

}