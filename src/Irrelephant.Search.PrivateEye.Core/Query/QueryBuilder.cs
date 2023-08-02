using System.Linq.Expressions;
using Azure.Search.Documents.Models;
using Irrelephant.Search.PrivateEye.Core.Search;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;
using Irrelephant.Search.PrivateEye.Core.Translation;
using SearchField = Irrelephant.Search.PrivateEye.Core.Search.SearchField;

namespace Irrelephant.Search.PrivateEye.Core.Query;

public class QueryBuilder<TIndexDocument, TSearchParams, TFilterParams>
{
    private SearchQueryNode? _query;

    private readonly ISearchQueryTranslator _searchQueryTranslator;
    private readonly IQueryExecutor<TIndexDocument> _queryExecutor;

    public QueryBuilder(
        ISearchQueryTranslator searchQueryTranslator,
        IQueryExecutor<TIndexDocument> queryExecutor
    )
    {
        _searchQueryTranslator = searchQueryTranslator;
        _queryExecutor = queryExecutor;
    }

    public QueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Where(
        Expression<Func<TFilterParams, SearchIndexMatch>> predicate)
    {
        return this;
    }

    public QueryBuilder<TIndexDocument, TSearchParams, TFilterParams> Search(
        Expression<Func<TSearchParams, SearchIndexMatch>> predicate)
    {
        var actualExpression = GetActualExpression(predicate.Body);
        var convertedExpression = AnalyzeExpression(actualExpression);
        _query = _query is not null
            // Multiple calls to .Search should combine clauses with "AND"
            ? new SearchQueryNode(new AndNode(_query.Op, convertedExpression))
            : new SearchQueryNode(convertedExpression);

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
        // Implicit conversion to facilitate SearchField comparison to its underlying type
        if (expression is UnaryExpression { NodeType: ExpressionType.Convert } convert)
        {
            expression = convert.Operand;
        }

        if (expression is MemberExpression memberExpression)
        {
            return new FieldNode(memberExpression.Member.Name);
        }

        return GetMatchValue(expression);
    }

    private ExpressionNode AnalyzeMethodCall(MethodCallExpression methodCall)
    {
        var methodTargetType = methodCall.Object?.Type;
        if (!ManagedTargetType(methodTargetType))
        {
            throw new NotSupportedException(
                "Can't call arbitrary methods in expressions. " +
                $"Only members of {nameof(SearchField)} are usable in this context."
            );
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

    private bool ManagedTargetType(Type? targetType) =>
        targetType is not null
        && typeof(SearchField).IsAssignableFrom(targetType);

    private TerminalNode GetMatchValue(Expression expression) =>
        expression switch
        {
            ConstantExpression constant => AnalyzeConstantAccessForMatch(constant),
            _ => AnalyzeWithCompilation(expression)
        };

    private TerminalNode AnalyzeWithCompilation(Expression expr)
    {
        // This is a potentially expensive way to obtain the result of an expression
        // More performance could be gained by analyzing the actual shape.
        var result = Expression.Lambda(expr).Compile().DynamicInvoke();
        return (TerminalNode)Activator.CreateInstance(
            typeof(ValueNode<>).MakeGenericType(expr.Type),
            result
        );
    }

    private TerminalNode AnalyzeConstantAccessForMatch(ConstantExpression constant) =>
        constant.Value switch
        {
            string s => new ValueNode<string>(s),
            int i32 => new ValueNode<int>(i32),
            float f32 => new ValueNode<float>(f32),
            double f64 => new ValueNode<double>(f64),
            bool b => new ValueNode<bool>(b),
            _ => throw new NotImplementedException("Unknown constant value")
        };

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
        return _queryExecutor.ExecuteAsync(stringQuery);
    }

    private string TranslateQuery() =>
        _query switch
        {
            null => _searchQueryTranslator.Translate(new SearchQueryNode()),
            _ => _searchQueryTranslator.Translate(_query)
        };
}
