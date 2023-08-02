namespace Irrelephant.Search.PrivateEye.Core.Search;

public record SearchField
{
    public bool Matches(string pattern) =>
        throw new InvalidOperationException(
            $"This method should only be used in Search query building. Pattern: {pattern}"
        );
}

// ReSharper disable once UnusedTypeParameter
public record SearchField<TField> : SearchField;
