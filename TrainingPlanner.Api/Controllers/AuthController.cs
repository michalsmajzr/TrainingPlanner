using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;
using TrainingPlanner.Application.Auth.Commands;

namespace TrainingPlanner.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController(
    ICommandHandler<RegisterUserCommand, AuthUser> registerHandler,
    ICommandHandler<LoginUserCommand, AuthUser> loginHandler,
    IIdentityService identityService,
    IAntiforgery antiforgery) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet("csrf")]
    public IActionResult GetCsrfToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { requestToken = tokens.RequestToken });
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        Result<AuthUser> result = await registerHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Bad Request");
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        Result<AuthUser> result = await loginHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status401Unauthorized,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Unauthorized");
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await identityService.SignOutAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        AuthUser? user = await identityService.GetCurrentUserAsync(User);

        if (user is null)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status401Unauthorized,
                errorCode: "auth.unauthorized",
                title: "Unauthorized");
        }

        return Ok(user);
    }
}

public sealed record RegisterRequest(
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@"^[^<>]*$")] string FirstName,
    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@"^[^<>]*$")] string LastName);

public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required, StringLength(128)] string Password);
