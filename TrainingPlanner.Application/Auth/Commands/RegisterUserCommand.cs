using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;

namespace TrainingPlanner.Application.Auth.Commands
{
    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName);

    public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthUser>
    {
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public Task<Result<AuthUser>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return _identityService.RegisterAsync(
                command.Email,
                command.Password,
                command.FirstName,
                command.LastName,
                cancellationToken);
        }
    }
}
