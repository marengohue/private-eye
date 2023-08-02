namespace Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

public class SearchQueryNode : UnaryNode
{
    public SearchQueryNode(ExpressionNode op) : base(op)
    {
    }

    public SearchQueryNode() : base(new WildcardNode()) { }

    public override bool Equals(object obj) =>
        obj is SearchQueryNode q && q.Op.Equals(Op);

    public override int GetHashCode() => (nameof(SearchQueryNode), Op).GetHashCode();
}

public class DocumentNode : TerminalNode
{
    public override bool Equals(object obj) => obj is DocumentNode;
    public override int GetHashCode() => nameof(DocumentNode).GetHashCode();
}

public class FieldNode : TerminalNode
{
    public string Name { get; }

    public FieldNode(string name)
    {
        Name = name;
    }

    public override bool Equals(object obj) => obj is FieldNode f && f.Name.Equals(Name);

    public override int GetHashCode() => (nameof(FieldNode), Name).GetHashCode();
}

public class ValueNode<TValueType> : TerminalNode
{
    public TValueType Value { get; }

    public ValueNode(TValueType value)
    {
        Value = value;
    }

    public override bool Equals(object obj) =>
        obj is ValueNode<TValueType> { Value: not null } v && v.Value.Equals(Value);

    public override int GetHashCode() => (nameof(ValueNode<TValueType>), Value).GetHashCode();
}

public class WildcardNode : TerminalNode
{
    public override bool Equals(object obj)
        => obj is WildcardNode;

    public override int GetHashCode() => nameof(WildcardNode).GetHashCode();
}

public class MatchNode : BinaryNode
{
    public MatchNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is MatchNode match && match.Left.Equals(Left) && match.Right.Equals(Right);

    public override int GetHashCode() => (nameof(MatchNode), Left, Right).GetHashCode();
}

public class StrictMatchNode : BinaryNode
{
    public StrictMatchNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj)
        => obj is StrictMatchNode match && match.Left.Equals(Left) && match.Right.Equals(Right);

    public override int GetHashCode() => (nameof(StrictMatchNode), Left, Right).GetHashCode();
}

public class AndNode : BinaryNode
{
    public AndNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is AndNode and && and.Left.Equals(Left) && and.Right.Equals(Right);

    public override int GetHashCode() => (nameof(AndNode), Left, Right).GetHashCode();
}

public class OrNode : BinaryNode
{
    public OrNode(Node left, Node right) : base(left, right)
    {
    }

    public override bool Equals(object obj) =>
        obj is OrNode and && and.Left.Equals(Left) && and.Right.Equals(Right);

    public override int GetHashCode() => (nameof(OrNode), Left, Right).GetHashCode();
}

public class NotNode : UnaryNode
{
    public NotNode(Node op) : base(op)
    {
    }

    public override bool Equals(object obj) =>
        obj is NotNode and && and.Op.Equals(Op);

    public override int GetHashCode() => (nameof(NotNode), Op).GetHashCode();
}
