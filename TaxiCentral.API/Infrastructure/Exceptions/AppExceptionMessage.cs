namespace TaxiCentral.API.Infrastructure.Exceptions
{
    public class AppExceptionMessage
    {
        public static string MISSING_AUTHENTICATION_SECRET = "MissingAuthenticationSecretException";
        public static string MISSING_AUTHENTICATION_ISSUER = "MissingAuthenticationIssuerException";
        public static string MISSING_AUTHENTICATION_AUDIENCE = "MissingAuthenticationAudienceException";
        public static string INVALID_TOKEN_MISSING_SUB = "InvalidTokenMissingSub";
    }
}
