using System.Linq.Expressions;

namespace Its.Search.PrivateEye.Core.Query;

public class SearchQueryBuilder<TIndexDocument, TParameters>
{
    public SearchQueryBuilder<TIndexDocument, TParameters> Where(
        Expression<Func<TParameters, SearchIndexMatch>> predicate)
    {
        return this;
    }

    public Task<TIndexDocument[]> ToArrayAsync()
    {
        return Task.FromResult(Array.Empty<TIndexDocument>());
    }
}
