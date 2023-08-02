using System.Linq.Expressions;
using Azure.Search.Documents.Models;
using Irrelephant.Search.PrivateEye.Core.Search;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Core.Translation;
using SearchField = Irrelephant.Search.PrivateEye.Core.Search.SearchField;

namespace Irrelephant.Search.PrivateEye.Core.Query;

public class SearchQueryBuilder<TIndexDocument, TSearchParams, TFilterParams>
{
    private QueryNode? _query;

    private readonly ISearchQueryTranslator _searchQueryTranslator;
    private readonly ISearchQueryExecutor<TIndexDocument> _searchQueryExecutor;

    public SearchQueryBuilder(
        ISearchQueryTranslator searchQueryTranslator,
        ISearchQueryExecutor<TIndexDocument> searchQueryExecutor
    )
    {
        _searchQueryTranslator = searchQueryTranslator;
        _searchQueryExecutor = searchQueryExecutor;
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
        _query = new QueryNode(AnalyzeExpression(actualExpression));
        return this;
    }

    private ExpressionNode AnalyzeExpression(Expression e) =>
        e switch {
            MethodCallExpression mc => AnalyzeMethodCall(mc),
            BinaryExpression { NodeType: ExpressionType.Equal } eq => AnalyzeEquals(eq),
            BinaryExpression { NodeType: ExpressionType.AndAlso or ExpressionType.And } and => AnalyzeAnd(and),
            BinaryExpression { NodeType: ExpressionType.OrElse or ExpressionType.Or } or => AnalyzeOr(or),
            UnaryExpression { NodeType: ExpressionType.Not } not => AnalyzeNot(not),
            _ => throw new NotSupportedException("Expression not supported!")
        };

    private ExpressionNode AnalyzeEquals(BinaryExpression binary) =>
        new StrictMatchNode(
            GetNodeFromExpression(binary.Left),
            GetNodeFromExpression(binary.Right)
        );

    private ExpressionNode AnalyzeAnd(BinaryExpression binaryExpression) =>
        new AndNode(
            AnalyzeExpression(binaryExpression.Left),
            AnalyzeExpression(binaryExpression.Right)
        );

    private ExpressionNode AnalyzeOr(BinaryExpression binaryExpression) =>
        new OrNode(
            AnalyzeExpression(binaryExpression.Left),
            AnalyzeExpression(binaryExpression.Right)
        );

    private ExpressionNode AnalyzeNot(UnaryExpression unaryExpression) =>
        new NotNode(AnalyzeExpression(unaryExpression.Operand));

    private TerminalNode GetNodeFromExpression(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
        {
            return new FieldNode(memberExpression.Member.Name);
        }

        return GetMatchValue(expression);
    }

    private ExpressionNode AnalyzeMethodCall(MethodCallExpression methodCall)
    {
        var methodTargetType = methodCall.Object?.Type;
        if (!FittingTargetType(methodTargetType))
        {
            throw new NotSupportedException("Can't call arbitrary methods in expressions just yet.");
        }

        if (methodCall.Method.Name != nameof(SearchField.Matches))
        {
            throw new NotSupportedException("Can't translate the expression tree. Unknown method called!");
        }

        if (methodCall.Object is MemberExpression memberExpression)
        {
            var matchValue = GetMatchValue(methodCall.Arguments.First());
            if (memberExpression.Type == typeof(FullTextSearchParameters))
            {
                return new MatchNode(new DocumentNode(), matchValue);
            }

            var memberName = memberExpression.Member.Name;
            return new MatchNode(new FieldNode(memberName), matchValue);
        }

        throw new NotImplementedException("Can't translate the expression tree. Odd shape innit?");
    }

    private bool FittingTargetType(Type? targetType) =>
        targetType is not null
        && (
            typeof(SearchField).IsAssignableFrom(targetType)
            || typeof(FullTextSearchParameters).IsAssignableFrom(targetType)
        );

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

    public Task<SearchResults<TIndexDocument>> ExecuteAsync()
    {
        var stringQuery = TranslateQuery();
        return _searchQueryExecutor.ExecuteAsync(stringQuery);
    }

    private string TranslateQuery() =>
        _query switch
        {
            null => _searchQueryTranslator.Translate(new QueryNode()),
            _ => _searchQueryTranslator.Translate(_query)
        };
}
