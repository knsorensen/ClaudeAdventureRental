using System.Text.Json.Serialization;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault (skipped locally when KeyVaultName is empty)
var kvName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrWhiteSpace(kvName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{kvName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication — Microsoft.Identity.Web validates Entra tokens
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

// Application services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdventureRental API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your Entra access token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddResponseCompression();

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdventureRental API v1"));
}

app.UseResponseCompression();

// Prevent browser from caching Blazor WASM assets in development so a
// page reload always picks up the latest build without needing Ctrl+Shift+R.
if (app.Environment.IsDevelopment())
{
    app.Use(async (ctx, next) =>
    {
        ctx.Response.OnStarting(() =>
        {
            if (ctx.Request.Path.StartsWithSegments("/_framework"))
            {
                ctx.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Response.Headers["Pragma"] = "no-cache";
            }
            return Task.CompletedTask;
        });
        await next();
    });
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
