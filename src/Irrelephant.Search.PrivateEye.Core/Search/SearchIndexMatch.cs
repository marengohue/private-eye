namespace Irrelephant.Search.PrivateEye.Core.Search;

public class SearchIndexMatch
{
    // ReSharper disable once UnusedParameter.Global
    // Implicit conversions exist to compare field values against constants
    // in a natural way. This does not work with booleans though.
    // TODO: Make it work with booleans
    public static implicit operator SearchIndexMatch(bool result) =>
        throw new InvalidOperationException("Search index match implicit cast should never be called in runtime.");
}
