using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;
using TrainingPlanner.Application.Auth.Commands;

namespace TrainingPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly ICommandHandler<RegisterUserCommand, AuthUser> _registerHandler;
        private readonly ICommandHandler<LoginUserCommand, AuthUser> _loginHandler;
        private readonly IIdentityService _identityService;
        private readonly IAntiforgery _antiforgery;

        public AuthController(
            ICommandHandler<RegisterUserCommand, AuthUser> registerHandler,
            ICommandHandler<LoginUserCommand, AuthUser> loginHandler,
            IIdentityService identityService,
            IAntiforgery antiforgery)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _identityService = identityService;
            _antiforgery = antiforgery;
        }

        [AllowAnonymous]
        [HttpGet("csrf")]
        public IActionResult GetCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
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

            Result<AuthUser> result = await _registerHandler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Description });
            }

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            Result<AuthUser> result = await _loginHandler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return Unauthorized(new { result.Error.Code, result.Error.Description });
            }

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _identityService.SignOutAsync();
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            AuthUser? user = await _identityService.GetCurrentUserAsync(User);
            return user is null ? Unauthorized() : Ok(user);
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
}
