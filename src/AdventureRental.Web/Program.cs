using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AdventureRental.Web;
using AdventureRental.Web.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<CustomAuthMessageHandler>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<CustomAuthMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
});

await builder.Build().RunAsync();
