namespace Conferenti.Domain.Abstractions;

public static class DomainErrors
{
    public static Error CreateError<T>(string description)
        where T : class
    {
        return new Error($"{typeof(T).Name}.Failure", description, ErrorType.Failure);
    }
}