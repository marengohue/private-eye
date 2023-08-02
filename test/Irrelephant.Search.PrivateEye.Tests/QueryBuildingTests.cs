using Irrelephant.Search.PrivateEye.Core;
using Irrelephant.Search.PrivateEye.Core.Query;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Tests.Mocks;
using Irrelephant.Search.PrivateEye.Tests.SearchInfrastructure;

namespace Irrelephant.Search.PrivateEye.Tests;

public class QueryBuildingTests
{
    private readonly ISearchService<SampleDocument, SampleDocumentSearchParameters, SampleDocumentFilterParameters> _searchService;

    private readonly MockQueryTranslator _searchTranslator =  new();

    public QueryBuildingTests()
    {
        var queryExecutorMock = new Mock<IQueryExecutor<SampleDocument>>();
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new AndNode(
                    new MatchNode(new FieldNode("SomeText"), new ValueNode<string>("Some")),
                    new MatchNode(new FieldNode("SomeOtherText"), new ValueNode<string>("Other"))
                )
            )
        );
    }

    [Fact]
    public async Task TestMultipleCalls_ResultInAnd()
    {
        await _searchService.Query()
            .Search(it => it.SomeText.Matches("Some"))
            .Search(it => it.SomeOtherText.Matches("Other"))
            .Search(it => it.FullText.Matches("Wow"))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new AndNode(
                    new AndNode(
                        new MatchNode(new FieldNode("SomeText"), new ValueNode<string>("Some")),
                        new MatchNode(new FieldNode("SomeOtherText"), new ValueNode<string>("Other"))
                    ),
                    new MatchNode(new DocumentNode(), new ValueNode<string>("Wow"))
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new OrNode(
                    new MatchNode(new FieldNode("SomeText"), new ValueNode<string>("Some")),
                    new MatchNode(new FieldNode("SomeOtherText"), new ValueNode<string>("Other"))
                )
            )
        );
    }

    private readonly string _someOtherText = "Other";
    private const string SomeConst = "Const";
    private string SomeProp { get; set; } = "Prop";

    [Fact]
    public async Task Test_VariableAndPropertyAccess_InExpressions()
    {
        var someText = "Some";
        await _searchService.Query()
            .Search(it => it.FullText.Matches(someText))
            .Search(it => it.FullText.Matches(_someOtherText))
            .Search(it => it.FullText.Matches(SomeConst))
            .Search(it => it.FullText.Matches(SomeProp))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new AndNode(
                    new AndNode(
                        new AndNode(
                            new MatchNode(new DocumentNode(), new ValueNode<string>("Some")),
                            new MatchNode(new DocumentNode(), new ValueNode<string>("Other"))
                        ),
                        new MatchNode(new DocumentNode(), new ValueNode<string>("Const"))
                    ),
                    new MatchNode(new DocumentNode(), new ValueNode<string>("Prop"))
                )
            )
        );
    }

    private string GetSomeText() => "Func";
    private string GetSomeText(string other) => "Woah" + other;
    private static string GetStaticText() => "Lol";

    [Fact]
    private async Task TestFunctionCalls_InExpressions()
    {
        await _searchService.Query()
            .Search(it => it.FullText.Matches(GetSomeText() + GetStaticText()))
            .Search(it => it.FullText.Matches(GetSomeText()))
            .Search(it => it.FullText.Matches(GetSomeText("Huh")))
            .ExecuteAsync();

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new AndNode(
                    new AndNode(
                        new MatchNode(new DocumentNode(), new ValueNode<string>("FuncLol")),
                        new MatchNode(new DocumentNode(), new ValueNode<string>("Func"))
                    ),
                    new MatchNode(new DocumentNode(), new ValueNode<string>("WoahHuh"))
                )

            )
        );
    }

    private class TestClass
    {
        public string Test() => "Test";
        public string Test2 => "Test";

        // TODO: Add async support
        // ReSharper disable once UnusedMember.Local
        public async Task<string> Test3()
        {
            await Task.Delay(TimeSpan.FromMicroseconds(1));
            return "Test";
        }
    }

    [Fact]
    private async Task Test_ComplexMatchResult()
    {
        var testClass = new TestClass();
        var guid = Guid.NewGuid();
        await _searchService.Query()
            .Search(it => it.FullText.Matches(guid.ToString("D") + testClass.Test2 + testClass.Test()))
            .Search(it => it.SomeText == testClass.Test())
            .ExecuteAsync();

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
                new AndNode(
                    new MatchNode(
                        new DocumentNode(),
                        new ValueNode<string>(guid.ToString("D") + "TestTest")
                    ),
                    new StrictMatchNode(new FieldNode("SomeText"), new ValueNode<string>("Test"))
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

        _searchTranslator.LastSubmittedSearchQuery.Should().Be(
            new SearchQueryNode(
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
