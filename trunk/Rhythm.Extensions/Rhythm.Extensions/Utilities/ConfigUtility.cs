namespace Rhythm.Extensions.Utilities
{
	using Enums;
	using System;
	using System.Configuration;
	public class ConfigUtility
	{
		public static string GetString(ConfigKeys key)
		{
			string value;
			switch (key)
			{
				case ConfigKeys.RecaptchaPrivateKey:
					value = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
					break;
				case ConfigKeys.RecaptchaPublicKey:
					value = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
					break;
				default:
					throw new InvalidOperationException("Unknown configuration key.");
			}
			return value;
		}

        public static bool GetBool(ConfigKeys key)
        {
            bool value;

            switch (key)
            {
                case ConfigKeys.ForceSSL:
                    value = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ForceSSL"]) && (ConfigurationManager.AppSettings["ForceSSL"].ToLower() == "true");
                    break;
                default:
                    throw new InvalidOperationException("Unknown configuration key.");
            }

            return value;
        }
	}
}