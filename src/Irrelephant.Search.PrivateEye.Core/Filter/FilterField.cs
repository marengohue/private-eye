namespace Irrelephant.Search.PrivateEye.Core.Filter;

public class FilterField<TField> : IEquatable<TField>, IEquatable<SimpleFilterField<TField>>
{
    // ReSharper disable once UnusedParameter.Global
    // Implicit conversions exist to compare field values against constants
    // in a natural way. This does not work with booleans though.
    // TODO: Make it work with booleans
    public static implicit operator TField(FilterField<TField> field) =>
        throw new InvalidOperationException("Filter field implicit cast should never be called in runtime.");

    public bool Equals(SimpleFilterField<TField> other) =>
        throw new InvalidOperationException("Filter field equals should never be called in runtime.");

    public bool Equals(TField? other) =>
        throw new InvalidOperationException("Filter field equals should never be called in runtime.");
}
