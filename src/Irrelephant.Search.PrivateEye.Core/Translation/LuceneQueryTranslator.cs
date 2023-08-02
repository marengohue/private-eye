using System.Text;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Core.Translation;

public class LuceneQueryTranslator : ISearchQueryTranslator
{
    private const char LuceneFieldSeparator = ':';
    private const char LuceneStringQuote = '"';
    private const char LuceneWildcard = '*';
    private const char LuceneOpenExpr = '(';
    private const char LuceneCloseExpr = ')';
    private const string LuceneAnd = " AND ";
    private const string LuceneOr = " OR ";
    private const string LuceneNot = "NOT ";

    public string Translate(SearchQueryNode node)
    {
        var builder = new StringBuilder();
        TraverseNode(builder, node.Op);
        return builder.ToString();
    }

    private void TraverseNode(StringBuilder builder, Node node)
    {
        if (node is WildcardNode)
        {
            builder.Append(LuceneWildcard);
            return;
        }

        if (node is MatchNode match)
        {
            TraverseLeftMatch(builder, match.Left);
            TraverseRightMatch(builder, match.Right);
            return;
        }

        if (node is StrictMatchNode strictMatch)
        {
            TraverseLeftMatch(builder, strictMatch.Left);
            TraverseRightMatch(builder, strictMatch.Right, isStrict: true);
            return;
        }

        if (node is AndNode and)
        {
            builder.Append(LuceneOpenExpr);
            TraverseNode(builder, and.Left);
            builder.Append(LuceneAnd);
            TraverseNode(builder, and.Right);
            builder.Append(LuceneCloseExpr);
            return;
        }

        if (node is OrNode or)
        {
            builder.Append(LuceneOpenExpr);
            TraverseNode(builder, or.Left);
            builder.Append(LuceneOr);
            TraverseNode(builder, or.Right);
            builder.Append(LuceneCloseExpr);
            return;
        }

        if (node is NotNode not)
        {
            builder.Append(LuceneNot);
            TraverseNode(builder, not.Op);
            return;
        }

        throw new NotImplementedException(
            $"Query translation not implemented for this node: {node}"
        );
    }

    private void TraverseRightMatch(StringBuilder builder, Node matchRight, bool isStrict = false)
    {
        if (matchRight is ValueNode<string> stringField)
        {
            builder.Append(LuceneStringQuote);
            builder.Append(stringField.Value);
            if (!isStrict)
            {
                builder.Append(LuceneWildcard);
            }
            builder.Append(LuceneStringQuote);

            return;
        }

        throw new NotImplementedException(
            $"Couldn't translate the right part of the match expr for {matchRight}"
        );
    }

    private void TraverseLeftMatch(StringBuilder builder, Node matchLeft)
    {
        if (matchLeft is FieldNode field)
        {
            builder.Append(field.Name);
            builder.Append(LuceneFieldSeparator);
            return;
        }

        if (matchLeft is DocumentNode)
        {
            // In case of full-text document match, its a NOOP since default
            // field doesn't have to be specified in the query.
            return;
        }

        throw new NotImplementedException(
            $"Couldn't translate the left part of the match expr for {matchLeft}"
        );
    }
}
