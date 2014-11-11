namespace Rhythm.Extensions.Utilities
{
	using Enums;
	using System;
	using System.Configuration;
	public class ConfigUtility {
		public static string GetString(ConfigKeys key) {
			string value;
			switch (key) {
				case ConfigKeys.RecaptchaPrivateKey:
					value = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
					break;
				case ConfigKeys.RecaptchaPublicKey:
					value = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
					break;
				default:
					throw new InvalidOperationException("Unknown string configuration key.");
			}
			return value;
		}

		public static bool GetBool(ConfigKeys key) {
			bool value;
			switch (key) {
				case ConfigKeys.ForceSSL:
					value = GetBoolForKey("ForceSSL");
					break;
				case ConfigKeys.ForceHttp:
					value = GetBoolForKey("ForceHTTP");
					break;
				case ConfigKeys.BypassLocalization:
					value = GetBoolForKey("BypassLocalization");
					break;
				case ConfigKeys.DisableFragmentUrlProvider:
					value = GetBoolForKey("DisableFragmentUrlProvider");
					break;
				default:
					throw new InvalidOperationException("Unknown bool configuration key.");
			}
			return value;
		}

		private static bool GetBoolForKey(string strKey) {
			var strVal = (ConfigurationManager.AppSettings[strKey] ?? string.Empty).ToLower();
			return !string.IsNullOrWhiteSpace(strVal) && strVal == "true";
		}
	}
}