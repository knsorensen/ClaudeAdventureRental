using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace AdventureRental.Web.Auth;

public class OptionalBearerHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;

    public OptionalBearerHandler(IAccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var result = await _tokenProvider.RequestAccessToken();
        if (result.TryGetToken(out var token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

        return await base.SendAsync(request, cancellationToken);
    }
}
