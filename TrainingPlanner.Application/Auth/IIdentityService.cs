using System.Security.Claims;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Auth
{
    public interface IIdentityService
    {
        Task<Result<AuthUser>> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            CancellationToken cancellationToken);

        Task<Result<AuthUser>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);

        Task SignOutAsync();

        Task<AuthUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
    }
}
