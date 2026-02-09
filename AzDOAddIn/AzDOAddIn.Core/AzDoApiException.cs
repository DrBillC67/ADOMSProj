using System;

namespace AzDOAddIn.Core
{
    /// <summary>
    /// Thrown when an Azure DevOps REST API request fails.
    /// </summary>
    public class AzDoApiException : Exception
    {
        public System.Net.HttpStatusCode? StatusCode { get; }
        public string ResponseBody { get; }

        public AzDoApiException(string message, System.Net.HttpStatusCode? statusCode = null, string responseBody = null)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public AzDoApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
