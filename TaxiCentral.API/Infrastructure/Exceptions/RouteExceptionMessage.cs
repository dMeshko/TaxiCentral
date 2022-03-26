namespace TaxiCentral.API.Infrastructure.Exceptions
{
    public class RouteExceptionMessage
    {
        public static string ALREADY_COMPLETE = "RouteAlreadyCompleteException";
        public static string NOT_FOUND = "RouteNotFoundException";
        public static string NO_RIDES = "NoRoutesFoundException";
    }
}
