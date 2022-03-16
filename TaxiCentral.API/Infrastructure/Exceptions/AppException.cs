using System.Net;

namespace TaxiCentral.API.Infrastructure.Exceptions
{
    public class AppException : Exception
    {
        public bool IsJson { get; protected set; }

        public AppException(string message) : base(message)
        {

        }
    }
}
