using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Core.Translation;

namespace Irrelephant.Search.PrivateEye.Tests;

public class LuceneTranslatorTests
{
    private readonly ISearchQueryTranslator _queryTranslator;

    public LuceneTranslatorTests()
    {
        _queryTranslator = new LuceneQueryTranslator();
    }

    [Fact]
    public void EmptyQuery_TranslatesToWildcard()
    {
        _queryTranslator
            .Translate(new QueryNode())
            .Should().Be("*");
    }

    [Fact]
    public void DocumentMatch_TranslatesToClause_WithoutPrefix()
    {
        _queryTranslator
            .Translate(
                new QueryNode(
                    new MatchNode(
                        new DocumentNode(),
                        new ValueNode<string>("value")
                    )
                )
            )
            .Should().Be("\"value*\"");
    }

    [Fact]
    public void DocumentMatch_MultipleTerms_AreCombinedWithAnd()
    {
        _queryTranslator
            .Translate(
                new QueryNode(
                    new AndNode(
                        new MatchNode(new DocumentNode(), new ValueNode<string>("value")),
                        new MatchNode(new DocumentNode(), new ValueNode<string>("another"))
                    )
                )
            )
            .Should().Be("(\"value*\" AND \"another*\")");
    }

    [Fact]
    public void Match_MultipleTerms_AreCombinedWithOr()
    {
        _queryTranslator
            .Translate(
                new QueryNode(
                    new OrNode(
                        new MatchNode(new DocumentNode(), new ValueNode<string>("value")),
                        new MatchNode(new FieldNode("test"), new ValueNode<string>("another"))
                    )
                )
            )
            .Should().Be("(\"value*\" OR test:\"another*\")");
    }

    [Fact]
    public void Match_MultipleTerms_AreNested()
    {
        _queryTranslator
            .Translate(
                new QueryNode(
                    new OrNode(
                        new MatchNode(new DocumentNode(), new ValueNode<string>("value")),
                        new AndNode(
                            new MatchNode(new DocumentNode(), new ValueNode<string>("something")),
                            new NotNode(new StrictMatchNode(new FieldNode("test"), new ValueNode<string>("me")))
                        )
                    )
                )
            )
            .Should().Be("(\"value*\" OR (\"something*\" AND (NOT test:\"me\")))");
    }
}
