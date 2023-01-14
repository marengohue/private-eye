namespace Irrelephant.Search.PrivateEye.Core.Filter;

public class SearchableFilterField<TField> : FilterField<TField>
{
    public bool Matches(string clause) => true;
}
