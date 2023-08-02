using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Irrelephant.Search.PrivateEye.Core.Query;

public interface IQueryExecutor<TResult>
{
    Task<SearchResults<TResult>> ExecuteAsync(string searchQuery);
}

public class DefaultQueryExecutor<TResult> : IQueryExecutor<TResult>
{
    private readonly SearchClient _searchClient;

    public DefaultQueryExecutor(SearchClient searchClient)
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
