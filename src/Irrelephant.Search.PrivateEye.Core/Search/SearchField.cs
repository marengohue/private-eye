namespace Irrelephant.Search.PrivateEye.Core.Search;

public record SearchField<TField>
{
    public bool Matches(string _) => true;
}

