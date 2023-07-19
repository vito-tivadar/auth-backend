using System.Security.Claims;

namespace auth_backend.Services;

public class ClaimsService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    public string? GetClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext
            ?.User?.Claims.Where(c => c.Type == claimType)
            .FirstOrDefault()
            ?.Value;
    }
}
