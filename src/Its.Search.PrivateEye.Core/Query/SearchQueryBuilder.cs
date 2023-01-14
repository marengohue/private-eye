using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Azure.Search.Documents.Indexes.Models;

namespace Its.Search.PrivateEye.Core.Query;

public class SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams>
{
    public SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Where(
        Expression<Func<TFilterParams, SearchIndexMatch>> predicate)
    {
        return this;
    }

    public SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Search(
        Expression<Func<TSearchParams, SearchIndexMatch>> predicate)
    {
        return this;
    }

    public Task<TIndexDocument[]> ToArrayAsync()
    {
        return Task.FromResult(Array.Empty<TIndexDocument>());
    }
}
