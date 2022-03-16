namespace TaxiCentral.API.Infrastructure.Helpers
{
    /// <summary>
    /// Extensions helper class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Rewrites the response with the appropriate exception
        /// </summary>
        /// <param name="response">The response</param>
        /// <param name="message">The exception message</param>
        /// <param name="isJson">This flag indicated whether the content type should be set to application/json.  This is used for validation against the "JsonPatchDocument"</param>
        public static void AddApplicationError(this HttpResponse response, string message, bool isJson = false)
        {
            response.Headers.Add("Application-Error", message);
            // CORS
            response.Headers.Add("access-control-expose-headers", "Application-Error");

            if (isJson)
            {
                response.ContentType = "application/json";
            }
        }
    }
}
