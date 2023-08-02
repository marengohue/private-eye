using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Core.Translation;

namespace Irrelephant.Search.PrivateEye.Tests.Mocks;

public class MockQueryTranslator : ISearchQueryTranslator
{
    public QueryNode LastSubmittedQuery { get; private set; } = null!;

    public string Translate(QueryNode root)
    {
        LastSubmittedQuery = root;
        return string.Empty;
    }
}
