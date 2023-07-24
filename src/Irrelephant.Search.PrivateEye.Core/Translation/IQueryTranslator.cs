using Irrelephant.Search.PrivateEye.Core.SyntaxTree;

namespace Irrelephant.Search.PrivateEye.Core.Translation;

/// <summary>
/// Service responsible for converting abstract syntax trees built up by LINQ-like syntax into actual string queries.
/// </summary>
/// <typeparam name="TQueryNode">Root of the syntax tree.</typeparam>
public interface IQueryTranslator<in TQueryNode> where TQueryNode : Node
{
    /// <summary>
    /// Translates the syntax tree from root of TQueryNode into a string representation
    /// </summary>
    /// <param name="root">root of the syntax tree</param>
    /// <returns></returns>
    string Translate(TQueryNode root);
}

