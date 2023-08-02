using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Irrelephant.Search.PrivateEye.Core.Query;

public interface ISearchQueryExecutor<TResult>
{
    Task<SearchResults<TResult>> ExecuteAsync(string searchQuery);
}

public class DefaultSearchQueryExecutor<TResult> : ISearchQueryExecutor<TResult>
{
    private readonly SearchClient _searchClient;

    public DefaultSearchQueryExecutor(SearchClient searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<SearchResults<TResult>> ExecuteAsync(string searchQuery) =>
        await _searchClient.SearchAsync<TResult>(
            searchQuery,
            new SearchOptions
            {
                QueryType = SearchQueryType.Full,
                SearchMode = SearchMode.All
            }
        );
}
