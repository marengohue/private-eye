namespace Irrelephant.Search.PrivateEye.Core.Search;

public record SearchField
{
    public bool Matches(string pattern) =>
        throw new InvalidOperationException(
            $"This method should only be used in Search query building. Pattern: {pattern}"
        );
}

// ReSharper disable once UnusedTypeParameter
public record SearchField<TField> : SearchField
{
    // ReSharper disable once UnusedParameter.Global
    // Implicit conversions exist to compare field values against constants
    // in a natural way. This does not work with booleans though.
    // TODO: Make it work with booleans
    public static implicit operator TField(SearchField<TField> field) =>
        throw new InvalidOperationException("Filter field implicit cast should never be called in runtime.");
}
