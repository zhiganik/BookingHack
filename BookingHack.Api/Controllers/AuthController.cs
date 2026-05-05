using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Contracts.Responses;
using BookingHack.Application.Services;
using BookingHack.Domain.Constants;
using BookingHack.Domain.Enums;
using BookingHack.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingHack.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtService _jwtService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshValidator;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtService jwtService,
        RefreshTokenService refreshTokenService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator,
        IValidator<RefreshTokenRequest> refreshValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _refreshValidator = refreshValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var validation = await _loginValidator.ValidateAsync(model);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(await BuildAuthResponseAsync(user));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var validation = await _registerValidator.ValidateAsync(model);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing is not null)
            return Conflict(new { message = "Email already in use" });

        var newUser = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        var roleName = model.Role switch
        {
            RegisterRole.Company => Roles.Company,
            _ => Roles.Customer
        };
        await _userManager.AddToRoleAsync(newUser, roleName);

        return Ok(await BuildAuthResponseAsync(newUser));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        var validation = await _refreshValidator.ValidateAsync(model);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var rotated = await _refreshTokenService.ValidateAndRotateAsync(model.RefreshToken);
        if (rotated is null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        var user = await _userManager.FindByIdAsync(rotated.Value.userId);
        if (user is null)
            return Unauthorized(new { message = "User not found" });

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponse(accessToken, _jwtService.GetExpiry(), rotated.Value.newRawToken));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest model)
    {
        await _refreshTokenService.RevokeAsync(model.RefreshToken);
        return NoContent();
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateToken(user, roles);
        var refreshToken = await _refreshTokenService.CreateAsync(user.Id);
        return new AuthResponse(accessToken, _jwtService.GetExpiry(), refreshToken);
    }
}
