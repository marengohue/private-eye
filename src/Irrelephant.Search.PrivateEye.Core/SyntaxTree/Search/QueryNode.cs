using System.Collections;

namespace Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

public class QueryNode : UnaryNode
{
    public QueryNode(ExpressionNode op) : base(op)
    {
    }

    public QueryNode() : base(new WildcardNode()) { }

    public override bool Equals(object obj) =>
        obj is QueryNode q && q.Op.Equals(Op);
}

public class DocumentNode : TerminalNode
{
    public override bool Equals(object obj) => obj is DocumentNode;
}

public class FieldNode : TerminalNode
{
    public string Name { get; }

    public FieldNode(string name)
    {
        Name = name;
    }

    public override bool Equals(object obj) => obj is FieldNode f && f.Name.Equals(Name);
}

public class ValueNode<TValueType> : TerminalNode
{
    public TValueType Value { get; }

    public ValueNode(TValueType value)
    {
        Value = value;
    }

    public override bool Equals(object obj) =>
        obj is ValueNode<TValueType> v && v.Value is not null && v.Value.Equals(Value);
}

public class WildcardNode : TerminalNode
{
    public override bool Equals(object obj)
        => obj is WildcardNode;
}

public class MatchNode : BinaryNode
{
    public MatchNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is MatchNode match && match.Left.Equals(Left) && match.Right.Equals(Right);

}

public class StrictMatchNode : BinaryNode
{
    public StrictMatchNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj)
        => obj is StrictMatchNode match && match.Left.Equals(Left) && match.Right.Equals(Right);
}

public class AndNode : BinaryNode
{
    public AndNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is AndNode and && and.Left.Equals(Left) && and.Right.Equals(Right);
}

public class OrNode : BinaryNode
{
    public OrNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is OrNode and && and.Left.Equals(Left) && and.Right.Equals(Right);
}

public class NotNode : UnaryNode
{
    public NotNode(Node op) : base(op)
    {
    }

    public override bool Equals(object obj) =>
        obj is NotNode and && and.Op.Equals(Op);
}
