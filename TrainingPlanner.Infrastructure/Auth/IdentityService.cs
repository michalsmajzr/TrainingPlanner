using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;
using TrainingPlanner.Infrastructure.Identity;

namespace TrainingPlanner.Infrastructure.Auth
{
    public sealed class IdentityService : IIdentityService
    {
        private static readonly Error InvalidCredentials =
            new("Auth.InvalidCredentials", "Email nebo heslo nejsou správné.");

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<AuthUser>> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                return Result<AuthUser>.Failure(
                    new Error("Auth.InvalidRegistration", "Vyplňte všechna povinná pole."));
            }

            email = email.Trim();
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim()
            };

            IdentityResult createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                string description = string.Join(" ", createResult.Errors.Select(error => error.Description));
                return Result<AuthUser>.Failure(new Error("Auth.RegistrationFailed", description));
            }

            return Result<AuthUser>.Success(ToAuthUser(user));
        }

        public async Task<Result<AuthUser>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Result<AuthUser>.Failure(InvalidCredentials);
            }

            User? user = await _userManager.FindByEmailAsync(email.Trim());
            if (user is null)
            {
                return Result<AuthUser>.Failure(InvalidCredentials);
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                return Result<AuthUser>.Failure(InvalidCredentials);
            }

            return Result<AuthUser>.Success(ToAuthUser(user));
        }

        public Task SignOutAsync() => _signInManager.SignOutAsync();

        public async Task<AuthUser?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            User? user = await _userManager.GetUserAsync(principal);
            return user is null ? null : ToAuthUser(user);
        }

        private static AuthUser ToAuthUser(User user) =>
            new(user.Id, user.Email ?? string.Empty, user.FirstName, user.LastName);
    }
}
