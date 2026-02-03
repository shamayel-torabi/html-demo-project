using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GcPdfViewerSupportApiDemo
{

    /// <summary>
    /// The RequestVerifier class provides functionality for verifying and validating incoming requests.
    /// It includes methods to check the origin of Cross-Origin Resource Sharing (CORS) requests,
    /// ensuring they meet specified criteria, and performs other request-related validations.
    /// This class is designed to enhance the security and reliability of incoming requests,
    /// offering a centralized location for request verification logic within the application.
    /// </summary>
    public class RequestVerifier
    {

        // List of allowed CORS origins (example: * allows everything)
        private static readonly string[] CorsOriginAllowed = {
            "localhost", "127.0.0.1", "*.example.com", "*"
        };

        /// <summary>
        /// Checks if the specified host is allowed for CORS.
        /// </summary>
        /// <param name="host">The host to check.</param>
        /// <returns>True if the host is allowed; otherwise, false.</returns>
        public static bool IsOriginAllowed(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return false;

            if (!Uri.TryCreate(host, UriKind.Absolute, out var uri))
                return false;

            var allowed = CorsOriginAllowed.Any(origin =>
                Regex.IsMatch(uri.Host, $"^{Regex.Escape(origin).Replace("\\*", ".*")}(?:\\.[a-z]+)?$", RegexOptions.IgnoreCase));
            return allowed;
        }

        /// <summary>
        /// Validates the authenticity and expiration of an authentication token associated with an incoming request.
        /// This method contributes to the security of the application by preventing unauthorized or tampered requests.
        /// </summary>
        /// <param name="request">The HTTP request associated with the authentication token.</param>
        /// <param name="token">The authentication token to be verified.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        public static bool VerifySecurityToken(HttpRequest request, string token)
        {
            // Implementation of token validation logic goes here
            // Return true if the token is valid; otherwise, return false
            // ...

            // Here is a simple example of how you can validate the authentication token
            // provided during viewer initialization - new DsPdfViewer(selector, { supportApi: { token: "support-api-demo" }});.
            /*
            if (string.IsNullOrEmpty(token) || !token.StartsWith("support-api-demo"))
            {
                return false;
            }
            */
            return true;
        }

    }
}
