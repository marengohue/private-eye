using Irrelephant.Search.PrivateEye.Core.SyntaxTree;
using Irrelephant.Search.PrivateEye.Core.SyntaxTree.Search;

namespace Irrelephant.Search.PrivateEye.Tests;

public static class Utility
{
    public static string PrintNode(Node? node) =>
        node switch
        {
            null => "<empty>",
            BinaryNode b => $"{PrintNodeName(b)}({PrintNode(b.Left)}, {PrintNode(b.Right)})",
            UnaryNode u => $"{PrintNodeName(u)}({PrintNode(u.Op)})",
            TerminalNode t => PrintTerminalNode(t),
            _ => throw new NotSupportedException("Unknown node type")
        };

    private static string PrintTerminalNode(TerminalNode node) =>
        node switch
        {
            FieldNode f => $"Field(\"{f.Name}\")",
            ValueNode<bool> b => $"Bool({b.Value})",
            ValueNode<int> i32 => $"Int({i32.Value})",
            ValueNode<float> f32 => $"Float({f32.Value})",
            ValueNode<double> f64 => $"Double({f64.Value})",
            ValueNode<string> s => $"String(\"{s.Value}\")",
            _ => throw new NotSupportedException("Unknown terminal node type")
        };

    private static string PrintNodeName(Node node) =>
        node.GetType()
            .Name
            .Replace("Node", string.Empty);
}
