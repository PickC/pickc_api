using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Development-only auth handler that auto-authenticates all requests
/// with a default CUSTOMER identity. Skips JWT validation entirely.
/// </summary>
public class DevBypassAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevBypassAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "DEV_USER"),
            new Claim("userType", "CUSTOMER"),
            new Claim("mobileNo", "9999999999"),
            new Claim(ClaimTypes.Role, "CUSTOMER"),
            new Claim(ClaimTypes.Role, "DRIVER")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
