using Irrelephant.Search.PrivateEye.Core;
using Irrelephant.Search.PrivateEye.Core.Query;
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
        var queryExecutorMock = new Mock<ISearchQueryExecutor<SampleDocument>>();
        queryExecutorMock
            .Setup(it => it.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null!);
        _searchService = new SampleDocumentSearchService(_searchTranslator, queryExecutorMock.Object);
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

            .ExecuteAsync();
    }

    [Fact]
    public async Task TestSimple_FieldMatchQuery()
    {
        await _searchService.Query()
            .Search(it => it.SomeText.Matches("Woah"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
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
    {
        await _searchService.Query()
            .Search(it => it.FullText.Matches("Woah"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
            new QueryNode(
                new MatchNode(
                    new DocumentNode(),
                    new ValueNode<string>("Woah")
                )
            )
        );
    }

    [Fact]
    public async Task TestSimple_EqualitySearch()
    {
        await _searchService.Query()
            .Search(it => it.SomeText == it.SomeOtherText)
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
            new QueryNode(
                new StrictMatchNode(
                    new FieldNode("SomeText"),
                    new FieldNode("SomeOtherText")
                )
            )
        );
    }

    [Fact]
    public async Task TestOperator_And()
    {
        await _searchService.Query()
            .Search(it => it.SomeText.Matches("Some") && it.SomeOtherText.Matches("Other"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
            new QueryNode(
                new AndNode(
                    new MatchNode(new FieldNode("SomeText"), new ValueNode<string>("Some")),
                    new MatchNode(new FieldNode("SomeOtherText"), new ValueNode<string>("Other"))
                )
            )
        );
    }

    [Fact]
    public async Task TestOperator_Or()
    {
        await _searchService.Query()
            .Search(it => it.SomeText.Matches("Some") || it.SomeOtherText.Matches("Other"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
            new QueryNode(
                new OrNode(
                    new MatchNode(new FieldNode("SomeText"), new ValueNode<string>("Some")),
                    new MatchNode(new FieldNode("SomeOtherText"), new ValueNode<string>("Other"))
                )
            )
        );
    }

    [Fact]
    public async Task TestOperator_Not()
    {
        await _searchService.Query()
            .Search(it => !it.SomeText.Matches("Some"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedQuery.Should().Be(
            new QueryNode(
                new NotNode(
                    new MatchNode(
                        new FieldNode("SomeText"),
                        new ValueNode<string>("Some")
                    )
                )
            )
        );
    }
}