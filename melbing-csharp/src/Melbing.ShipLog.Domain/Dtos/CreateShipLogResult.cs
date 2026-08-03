namespace Melbing.ShipLog.Domain.Dtos;

public sealed class CreateShipLogResult
{
    public bool IsValidationError { get; private init; }
    public IDictionary<string, string[]>? Errors { get; private init; }

    public static CreateShipLogResult Success() => new() { IsValidationError = false };

    public static CreateShipLogResult ValidationError(IDictionary<string, string[]> errors) => new()
    {
        IsValidationError = true,
        Errors = errors,
    };
}
