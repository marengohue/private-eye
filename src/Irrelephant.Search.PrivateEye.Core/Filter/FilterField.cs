namespace Irrelephant.Search.PrivateEye.Core.Filter;

public class FilterField<TField> : IEquatable<TField>, IEquatable<SimpleFilterField<TField>>
{
    // Implicit conversions exist to compare field values against constants
    // in a natural way. This does not work with booleans though.
    // TODO: Make it work with booleans
    public static implicit operator TField(FilterField<TField> _) => default!;

    public bool Equals(SimpleFilterField<TField> _) => true;

    public bool Equals(TField? _) => true;
}
