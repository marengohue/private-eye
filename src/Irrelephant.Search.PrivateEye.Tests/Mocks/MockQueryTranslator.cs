using Irrelephant.Search.PrivateEye.Core;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Tests.Mocks;

public class MockQueryTranslator : IQueryTranslator<QueryNode>
{
    public QueryNode LastSubmittedQuery { get; private set; } = null!;

    public string Translate(QueryNode root)
    {
        LastSubmittedQuery = root;
        return string.Empty;
    }
}
