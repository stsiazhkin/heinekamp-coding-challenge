using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Heinekamp.CodingChallenge.FileApi.Authentication;

public class ApiKeyAuthenticationHandler(
    ExternalApiUsers externalApiUsers,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
#pragma warning disable CS0618
    ISystemClock clock)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
#pragma warning restore CS0618
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthenticationConstants.ApiKeyHeaderName, out var headerStringValues))
            return AuthenticateResult.Fail("API key required");

        if (!AuthenticationHeaderValue.TryParse(headerStringValues, out var parsedHeaderValue))
            return AuthenticateResult.Fail("Provided API key invalid");

        if (!externalApiUsers.TryGetValue(parsedHeaderValue.Scheme, out var user))
            return AuthenticateResult.Fail("API key is not authorized");
        
        var claims = new[] { new Claim(ClaimTypes.Name, user) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
