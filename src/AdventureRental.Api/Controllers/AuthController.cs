using System.Security.Claims;
using AdventureRental.Core.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthResponse> Me()
    {
        var email = User.FindFirstValue("preferred_username")
            ?? User.FindFirstValue("email")
            ?? string.Empty;
        var firstName = User.FindFirstValue("given_name") ?? string.Empty;
        var lastName = User.FindFirstValue("family_name") ?? string.Empty;
        var roles = User.FindAll("roles").Select(c => c.Value).ToList();

        return Ok(new AuthResponse
        {
            Token = string.Empty,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles
        });
    }
}
