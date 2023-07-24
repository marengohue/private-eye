namespace Irrelephant.Search.PrivateEye.Core.Search;

public record SearchField
{
    public bool Matches(string _) => true;
}

public record SearchField<TField> : SearchField;

