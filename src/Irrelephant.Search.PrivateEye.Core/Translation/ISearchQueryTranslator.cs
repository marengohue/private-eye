using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Core.Translation;

/// <summary>
/// Marker interface for translators specific to the search engine.
/// </summary>
public interface ISearchQueryTranslator : IQueryTranslator<SearchQueryNode> { }
