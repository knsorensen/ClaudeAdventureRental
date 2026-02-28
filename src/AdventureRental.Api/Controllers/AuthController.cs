using AdventureRental.Core.DTOs.Auth;
using AdventureRental.Core.DTOs.Customer;
using AdventureRental.Core.Entities;
using AdventureRental.Core.Interfaces;
using AdventureRental.Infrastructure.Data;
using AdventureRental.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdventureRental.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly IUnitOfWork _uow;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        IUnitOfWork uow)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _uow = uow;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Conflict(new { message = "Email already registered." });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "Customer");

        var customer = new Core.Entities.Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserId = user.Id
        };
        await _uow.Customers.AddAsync(customer);
        await _uow.SaveChangesAsync();

        user.CustomerId = customer.Id;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new AuthResponse
        {
            Token = _tokenService.CreateToken(user, roles),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new AuthResponse
        {
            Token = _tokenService.CreateToken(user, roles),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AuthResponse>> Me()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        if (user == null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new AuthResponse
        {
            Token = string.Empty,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }
}
