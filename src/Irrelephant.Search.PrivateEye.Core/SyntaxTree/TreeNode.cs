namespace Irrelephant.Search.PrivateEye.Core.SyntaxTree;

public abstract class Node
{
}

public abstract class ExpressionNode : Node
{
}

public abstract class BinaryNode : ExpressionNode
{
    public Node Left { get; }
    public Node Right { get; }

    protected BinaryNode(Node left, Node right)
    {
        Left = left;
        Right = right;
    }
}

public abstract class UnaryNode : ExpressionNode
{
    public Node Op { get; }

    protected UnaryNode(Node op)
    {
        Op = op;
    }
}

public abstract class TerminalNode : Node
{
}
