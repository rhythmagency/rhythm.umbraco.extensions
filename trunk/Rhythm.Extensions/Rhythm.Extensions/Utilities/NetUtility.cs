namespace Rhythm.Extensions.Utilities {

	// Namespaces.
	using Rhythm.Extensions.Enums;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Web;

	/// <summary>
	/// Utility to help with network operations.
	/// </summary>
	public class NetUtility {
		private const string DEFAULT_USER_AGENT = ".Net Server-Side Client";

		/// <summary>
		/// Posts a form and gets the response string.
		/// </summary>
		/// <param name="url">The URL to post the form data to.</param>
		/// <param name="data">The form data to post (key/value pairs).</param>
		/// <returns>The response string.</returns>
		public static string GetPostResponse(string url, List<KeyValuePair<string, string>> data) {
			var request = (HttpWebRequest) System.Net.WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.UserAgent = DEFAULT_USER_AGENT;
			var postData = new StringBuilder();
			string ampersand = string.Empty;
			foreach (var pair in data) {
				postData.Append(ampersand);
				postData.Append(pair.Key).Append("=");
				postData.Append(WebUtility.UrlEncode(pair.Value));
				ampersand = "&";
			}
			Encoding encoding = Encoding.UTF8;
			byte[] postBytes = encoding.GetBytes(postData.ToString());
			request.ContentLength = postBytes.Length;
			using (Stream postStream = request.GetRequestStream()) {
				postStream.Write(postBytes, 0, postBytes.Length);
				postStream.Flush();
				postStream.Close();
			}
			var response = (HttpWebResponse) request.GetResponse();
			var responseStream = response.GetResponseStream();
			var reader = new StreamReader(responseStream);
			return reader.ReadToEnd();
		}


		/// <summary>
		/// Returns the response string downloaded from the request to the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <returns>The response string.</returns>
		public static string GetResponse(string url) {
			var request = (HttpWebRequest) System.Net.WebRequest.Create(url);
			request.UserAgent = DEFAULT_USER_AGENT;
			var response = (HttpWebResponse) request.GetResponse();
			var responseStream = response.GetResponseStream();
			var reader = new StreamReader(responseStream);
			return reader.ReadToEnd();
		}

		/// <summary>
		/// Forces a 301 redirect to HTTPS if configured in appSettings.
		/// </summary>
		public static void ForceSSL() {
			var context = HttpContext.Current;

			if (!context.Request.IsSecureConnection && ConfigUtility.GetBool(ConfigKeys.ForceSSL)) {
				var builder = new UriBuilder(new Uri(context.Request.Url, context.Request.RawUrl)) {
					Scheme = Uri.UriSchemeHttps
				};

				// Strip default HTTP port.
				if (builder.Port == 80) {
					builder.Port = -1;
				}

				context.Response.Redirect(builder.Uri.AbsoluteUri);
			}
		}

		/// <summary>
		/// Forces a 301 redirect to HTTP if configured in appSettings.
		/// </summary>
		public static void ForceHttp() {
			var context = HttpContext.Current;

			if (context.Request.IsSecureConnection && ConfigUtility.GetBool(ConfigKeys.ForceHttp)) {
				var builder = new UriBuilder(new Uri(context.Request.Url, context.Request.RawUrl)) {
					Scheme = Uri.UriSchemeHttp
				};

				// Strip default HTTPS port.
				if (builder.Port == 443) {
					builder.Port = -1;
				}
				
				context.Response.Redirect(builder.Uri.AbsoluteUri);
			}
		}
	}

}