using Its.Search.PrivateEye.Core;
using Its.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Its.Search.PrivateEye.Tests;

public class SearchServiceGeneratorTests
{
    private readonly ISearchService<SampleDocument, SampleDocumentSearchParameters, SampleDocumentFilterParameters> _searchService;

    public SearchServiceGeneratorTests()
    {
        _searchService = new SampleDocumentSearchService();
    }

    [Fact]
    public async Task TestQuerying()
    {
        await _searchService.Query()
            .Search(it => it.FullText.Matches("coffee*"))
            .Search(it => it.SomeText.Matches("john*"))

            .Where(it => it.SomeText.Matches("text"))
            .Where(it => it.SomeNumber >= 20.0)
            .Where(it => it.SomeFloat <= 3f)
            .Where(it => it.SomeBoolean == true)
            .Where(it => it.Id == "woah")

            .ToArrayAsync();
    }

    [Fact]
    public async Task TestReferenceFiltering()
    {
        await _searchService.Query()
            .Where(it => it.SomeNumber == it.SomeOtherNumber)
            .ToArrayAsync();
    }
}
