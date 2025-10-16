using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Exceptions;
using Conferenti.Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;

namespace Conferenti.Application.Abstractions.Behaviors;
internal static class QueryValidationDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            var validationFailures = await ValidateAsync(query, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(query, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }

        private static async Task<ValidationFailure[]> ValidateAsync<TQueryRequest>(
            TQueryRequest query,
            IEnumerable<IValidator<TQueryRequest>> validators)
        {
            if (!validators.Any())
            {
                return [];
            }

            var context = new ValidationContext<TQueryRequest>(query);

            var validationResults = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context)));

            var validationFailures = validationResults
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .ToArray();

            return validationFailures;
        }

        private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
            new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
    }
}
