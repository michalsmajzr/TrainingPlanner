using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Auth;

namespace TrainingPlanner.Application.Auth.Commands
{
    public sealed record LoginUserCommand(string Email, string Password);

    public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthUser>
    {
        private readonly IIdentityService _identityService;

        public LoginUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public Task<Result<AuthUser>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            return _identityService.LoginAsync(command.Email, command.Password, cancellationToken);
        }
    }
}
