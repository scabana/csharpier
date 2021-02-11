using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpier.Core
{
    public partial class Printer
    {
        // TODO trivia more in here, this is kind of a mess
        private Doc PrintBasePropertyDeclarationSyntax(BasePropertyDeclarationSyntax node)
        {
            EqualsValueClauseSyntax initializer = null;
            ArrowExpressionClauseSyntax expressionBody = null;
            Doc identifier = null;
            Doc eventKeyword = null;
            if (node is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                expressionBody = propertyDeclarationSyntax.ExpressionBody;
                initializer = propertyDeclarationSyntax.Initializer;
                identifier = this.PrintSyntaxToken(propertyDeclarationSyntax.Identifier);
            }
            else if (node is IndexerDeclarationSyntax indexerDeclarationSyntax)
            {
                expressionBody = indexerDeclarationSyntax.ExpressionBody;
                identifier = Concat(
                    this.PrintSyntaxToken(indexerDeclarationSyntax.ThisKeyword),
                    "[",
                    Join(", ", indexerDeclarationSyntax.ParameterList.Parameters.Select(this.PrintParameterSyntax)),
                    "]"
                );
            }
            else if (node is EventDeclarationSyntax eventDeclarationSyntax)
            {
                eventKeyword = this.PrintSyntaxToken(eventDeclarationSyntax.EventKeyword, " ");
                identifier = this.PrintSyntaxToken(eventDeclarationSyntax.Identifier);
            }

            Doc contents = "";
            if (node.AccessorList != null)
            {
                contents = Group(Concat(Line, "{", Group(Indent(node.AccessorList.Accessors.Select(this.PrintAccessorDeclarationSyntax).ToArray())), Line, "}"));
            }
            else if (expressionBody != null)
            {
                contents = Concat(this.PrintArrowExpressionClauseSyntax(expressionBody), ";");
            }


            var parts = new Parts();

            parts.Push(this.PrintExtraNewLines(node));
            parts.Push(this.PrintAttributeLists(node, node.AttributeLists));

            return Group(
                Concat(
                    Concat(parts),
                    this.PrintModifiers(node.Modifiers),
                    eventKeyword,
                    this.Print(node.Type),
                    " ",
                    identifier,
                    contents,
                    initializer != null
                        ? Concat(this.PrintEqualsValueClauseSyntax(initializer), ";")
                        : ""
                )
            );
        }
    }
}