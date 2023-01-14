using Its.Search.PrivateEye.Core;
using Its.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Its.Search.PrivateEye.Tests;

public class SearchServiceGeneratorTests
{
    private readonly ISearchService<SampleDocument, SampleDocumentSearchParameters> _searchService;

    public SearchServiceGeneratorTests()
    {
        _searchService = new SampleDocumentSearchService();
    }

    [Fact]
    public async Task TestQuerying()
    {
        await _searchService.Query()
            .Where(it => it.SomeNumber == 10)
            .ToArrayAsync();
    }
}
