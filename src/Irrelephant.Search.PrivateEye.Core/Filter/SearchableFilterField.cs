namespace Irrelephant.Search.PrivateEye.Core.Filter;

public class SearchableFilterField<TField> : FilterField<TField>
{
    // ReSharper disable once UnusedParameter.Global
    public bool Matches(string clause) => true;
}
