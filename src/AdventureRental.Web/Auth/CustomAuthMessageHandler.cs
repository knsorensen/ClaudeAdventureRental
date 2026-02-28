using System.Net.Http.Headers;

namespace AdventureRental.Web.Auth;

public class CustomAuthMessageHandler : DelegatingHandler
{
    private readonly JwtAuthStateProvider _authStateProvider;

    public CustomAuthMessageHandler(JwtAuthStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authStateProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
