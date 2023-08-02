using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Core.Translation;

namespace Irrelephant.Search.PrivateEye.Tests.Mocks;

public class MockQueryTranslator : ISearchQueryTranslator
{
    public SearchQueryNode LastSubmittedSearchQuery { get; private set; } = null!;

    public string Translate(SearchQueryNode root)
    {
        LastSubmittedSearchQuery = root;
        return string.Empty;
    }
}
