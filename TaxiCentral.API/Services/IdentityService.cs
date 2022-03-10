namespace TaxiCentral.API.Services
{
    public interface IIdentityService
    {
        Guid GetUserId();
    }

    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContext;

        public IdentityService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public Guid GetUserId()
        {
            return new Guid(_httpContext.HttpContext!.User.FindFirst("sub")!.Value);
        }
    }
}
