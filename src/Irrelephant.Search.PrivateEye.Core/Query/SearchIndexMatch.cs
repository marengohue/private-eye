namespace Irrelephant.Search.PrivateEye.Core.Query;

public class SearchIndexMatch
{
    public static implicit operator SearchIndexMatch(bool _) => new();
}
