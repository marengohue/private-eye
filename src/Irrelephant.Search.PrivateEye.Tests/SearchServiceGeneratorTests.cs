using Irrelephant.Search.PrivateEye.Core;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Tests.Mocks;
using Irrelephant.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Irrelephant.Search.PrivateEye.Tests;

public class SearchServiceGeneratorTests
{
    private readonly ISearchService<SampleDocument, SampleDocumentSearchParameters, SampleDocumentFilterParameters> _searchService;

    private readonly MockQueryTranslator _searchTranslator =  new();

    public SearchServiceGeneratorTests()
    {
        _searchService = new SampleDocumentSearchService(_searchTranslator);
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
    public async Task TestSimple_FieldMatchQuery()
    {
        await _searchService.Query()
            .Search(it => it.SomeText.Matches("Woah"))
            .ToArrayAsync();

        _searchTranslator.LastSubmittedQuery.Should().BeEquivalentTo(
            new QueryNode(
                new MatchNode(
                    new FieldNode("SomeText"),
                    new ValueNode<string>("Woah")
                )
            )
        );
    }

    [Fact]
    public async Task TestSimple_DocumentMatchQuery()
    {        await _searchService.Query()
            .Search(it => it.FullText.Matches("Woah"))
            .ToArrayAsync();

        _searchTranslator.LastSubmittedQuery.Should().BeEquivalentTo(
            new QueryNode(
                new MatchNode(
                    new DocumentNode(),
                    new ValueNode<string>("Woah")
                )
            )
        );
    }

    [Fact]
    public async Task TestReferenceFiltering()
    {
        await _searchService.Query()
            .Where(it => it.SomeNumber == it.SomeOtherNumber)
            .ToArrayAsync();
    }
}
