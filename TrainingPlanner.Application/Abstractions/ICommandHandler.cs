namespace TrainingPlanner.Application.Abstractions
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
