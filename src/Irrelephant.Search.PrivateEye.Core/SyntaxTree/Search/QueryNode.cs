namespace Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

public class QueryNode : UnaryNode
{
    public QueryNode(ExpressionNode op) : base(op)
    {
    }

    public QueryNode() : base(new WildcardNode()) { }
}

public class DocumentNode : TerminalNode
{

}

public class FieldNode : TerminalNode
{
    public string Name { get; }

    public FieldNode(string name)
    {
        Name = name;
    }
}

public class ValueNode<TValueType> : TerminalNode
{
    public TValueType Value { get; }

    public ValueNode(TValueType value)
    {
        Value = value;
    }
}

public class WildcardNode : TerminalNode
{

}

public class MatchNode : BinaryNode
{
    public MatchNode(Node left, Node right) : base(left, right)
    {
    }
}
