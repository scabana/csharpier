using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpier.Core
{
    public partial class Printer
    {
        private Doc PrintLabeledStatementSyntax(LabeledStatementSyntax node)
        {
            var parts = new Parts();
            parts.Push(this.PrintAttributeLists(node, node.AttributeLists));
            parts.Push(node.Identifier.Text, ":");
            if (node.Statement is BlockSyntax blockSyntax) {
                parts.Push(this.PrintBlockSyntax(blockSyntax));
            } else {
                parts.Push(HardLine, this.Print(node.Statement));
            }
            return Concat(parts);
        }
    }
}
