namespace Its.Search.PrivateEye.Core.Search;

public record SearchField<TField>
{
    public bool Matches(string _) => true;
}

