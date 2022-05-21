namespace CSharpier.SyntaxPrinter;

internal static class RightHandSide
{
    public static Doc Print(
        CSharpSyntaxNode leftNode,
        Doc leftDoc,
        Doc operatorDoc,
        ExpressionSyntax rightNode,
        FormattingContext context
    )
    {
        var layout = DetermineLayout(leftNode, rightNode);

        var groupId = Guid.NewGuid().ToString();

        return layout switch
        {
            Layout.BasicConcatWithoutLine
              => Doc.Concat(leftDoc, operatorDoc, Node.Print(rightNode, context)),
            Layout.BreakAfterOperator
              => Doc.Group(
                  Doc.Group(leftDoc),
                  operatorDoc,
                  Doc.Group(Doc.Indent(Doc.Line, Node.Print(rightNode, context)))
              ),
            Layout.Chain
              => Doc.Concat(
                  Doc.Group(leftDoc),
                  operatorDoc,
                  Doc.Line,
                  Node.Print(rightNode, context)
              ),
            Layout.ChainTail
              => Doc.Concat(
                  Doc.Group(leftDoc),
                  operatorDoc,
                  Doc.Indent(Doc.Line, Node.Print(rightNode, context))
              ),
            Layout.Fluid
              => Doc.Group(
                  Doc.Group(leftDoc),
                  operatorDoc,
                  Doc.GroupWithId(groupId, Doc.Indent(Doc.Line)),
                  Doc.IndentIfBreak(Node.Print(rightNode, context), groupId)
              ),
            _ => throw new Exception("The layout type of " + layout + " was not handled.")
        };
    }

    private static Layout DetermineLayout(CSharpSyntaxNode leftNode, ExpressionSyntax rightNode)
    {
        var isTail = rightNode is not AssignmentExpressionSyntax;
        var shouldUseChainFormatting =
            leftNode is AssignmentExpressionSyntax
            && leftNode.Parent is AssignmentExpressionSyntax or EqualsValueClauseSyntax
            && (
                !isTail
                || leftNode.Parent.Parent
                    is not (
                        ExpressionStatementSyntax
                        or VariableDeclaratorSyntax
                        or ArrowExpressionClauseSyntax
                    )
            );

        if (shouldUseChainFormatting)
        {
            return !isTail ? Layout.Chain : Layout.ChainTail;
        }

        if (
            (
                !isTail
                && rightNode is AssignmentExpressionSyntax { Right: AssignmentExpressionSyntax }
            )
            || rightNode
                is ObjectCreationExpressionSyntax
                {
                    Type: GenericNameSyntax,
                    ArgumentList: { Arguments: { Count: 0 } },
                    Initializer: null
                }
        )
        {
            return Layout.BreakAfterOperator;
        }

        return rightNode switch
        {
            InitializerExpressionSyntax => Layout.BasicConcatWithoutLine,
            BinaryExpressionSyntax
            or CastExpressionSyntax { Type: GenericNameSyntax }
            or ConditionalExpressionSyntax
            {
                Condition: BinaryExpressionSyntax or ParenthesizedExpressionSyntax
            }
            or ImplicitObjectCreationExpressionSyntax { Parent: EqualsValueClauseSyntax }
            or InterpolatedStringExpressionSyntax
            // TODO ditch fluid?
            // or InvocationExpressionSyntax
            or IsPatternExpressionSyntax
            or LiteralExpressionSyntax
            // TODO ditch fluid?
            // or MemberAccessExpressionSyntax
            or StackAllocArrayCreationExpressionSyntax
            or QueryExpressionSyntax
              => Layout.BreakAfterOperator,
            _ => Layout.Fluid
        };
    }

    private enum Layout
    {
        BasicConcatWithoutLine,
        BreakAfterOperator,
        Chain,
        ChainTail,
        Fluid,
    }
}
