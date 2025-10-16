using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Domain.Abstractions;

namespace Conferenti.Application.Abstractions.Behaviors;
internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ////var commandName = typeof(TCommand).Name;

            ////logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            ////if (result.IsSuccess)
            ////{
            ////    logger.LogInformation("Completed command {Command}", commandName);
            ////}
            ////else
            ////{
            ////    using (LogContext.PushProperty("Error", result.Error, true))
            ////    {
            ////        logger.LogError("Completed command {Command} with error", commandName);
            ////    }
            ////}

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ////var commandName = typeof(TCommand).Name;

            ////logger.LogInformation("Processing command {Command}", commandName);

            var result = await innerHandler.Handle(command, cancellationToken);

            ////if (result.IsSuccess)
            ////{
            ////    logger.LogInformation("Completed command {Command}", commandName);
            ////}
            ////else
            ////{
            ////    using (LogContext.PushProperty("Error", result.Error, true))
            ////    {
            ////        logger.LogError("Completed command {Command} with error", commandName);
            ////    }
            ////}

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            ////var queryName = typeof(TQuery).Name;

            ////logger.LogInformation("Processing query {Query}", queryName);

            var result = await innerHandler.Handle(query, cancellationToken);

            ////if (result.IsSuccess)
            ////{
            ////    logger.LogInformation("Completed query {Query}", queryName);
            ////}
            ////else
            ////{
            ////    using (LogContext.PushProperty("Error", result.Error, true))
            ////    {
            ////        logger.LogError("Completed query {Query} with error", queryName);
            ////    }
            ////}

            return result;
        }
    }
}

