using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace AdventureRental.Web.Auth;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "authToken";
    private readonly IJSRuntime _js;
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public JwtAuthStateProvider(IJSRuntime js) => _js = js;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            if (string.IsNullOrWhiteSpace(token))
                return Anonymous;

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return Anonymous;

            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await ClearTokenAsync();
                return Anonymous;
            }

            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return Anonymous;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    public async Task<string?> GetTokenAsync() =>
        await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
}
