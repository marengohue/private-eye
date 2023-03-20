using System.Linq.Expressions;
using Azure.Search.Documents.Indexes.Models;
using Irrelephant.Search.PrivateEye.Core.Filter;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Core.Query;

public class SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams>
{
    private QueryNode? _query;

    private readonly IQueryTranslator<QueryNode> _searchQueryTranslator;

    public SearchQueryBuilder(IQueryTranslator<QueryNode> searchQueryTranslator)
    {
        _searchQueryTranslator = searchQueryTranslator;
    }

    public SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Where(
        Expression<Func<TFilterParams, SearchIndexMatch>> predicate)
    {
        return this;
    }

    public SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Search(
        Expression<Func<TSearchParams, SearchIndexMatch>> predicate)
    {
        var actualExpression = GetActualExpression(predicate.Body);
        if (actualExpression is MethodCallExpression methodCall)
        {
            if (methodCall.Method.Name == "Matches")
            {
                if (methodCall.Object is MemberExpression memberExpression)
                {
                    var memberName = memberExpression.Member.Name;
                    var matchValue = methodCall.Arguments.First();

                    AppendMatch(memberName, GetMatchValue(matchValue));
                }

            }
        }

        return this;
    }

    private TerminalNode GetMatchValue(Expression expression)
    {
        if (expression is ConstantExpression constant)
        {
            return constant.Value switch
            {
                string s => new ValueNode<string>(s),
                int i32 => new ValueNode<int>(i32),
                float f32 => new ValueNode<float>(f32),
                double f64 => new ValueNode<double>(f64),
                bool b => new ValueNode<bool>(b),
                _ => throw new NotImplementedException("Unknown constant value")
            };
        }

        throw new NotImplementedException("Not matching against a constant");
    }

    private void AppendMatch(string memberName, TerminalNode matchValue)
    {
        _query = new QueryNode(new MatchNode(new FieldNode(memberName), matchValue));
    }

    private Expression GetActualExpression(Expression predicate)
    {
        // For type safety, some of the top level expressions have an implicit conversion from Field<T> to T.
        // we unwrap the conversion before acting on the insides.
        if (predicate is UnaryExpression convert && predicate.NodeType == ExpressionType.Convert)
        {
            return convert.Operand;
        }

        return predicate;
    }

    public Task<TIndexDocument[]> ToArrayAsync()
    {
        var stringQuery = TranslateQuery();
        return Task.FromResult(Array.Empty<TIndexDocument>());
    }

    private string TranslateQuery() =>
        _query switch
        {
            null => _searchQueryTranslator.Translate(new QueryNode()),
            _ => _searchQueryTranslator.Translate(_query)
        };
}
