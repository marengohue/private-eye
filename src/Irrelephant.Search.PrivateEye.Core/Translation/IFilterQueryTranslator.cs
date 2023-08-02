using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Core.Translation;

/// <summary>
/// Marker interface for translators specific to OData filter queries.
/// </summary>
public interface IFilterQueryTranslator : IQueryTranslator<SearchQueryNode> { }
