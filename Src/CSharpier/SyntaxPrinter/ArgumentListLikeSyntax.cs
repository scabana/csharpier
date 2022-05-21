namespace CSharpier.SyntaxPrinter;

internal static class ArgumentListLike
{
    public static Doc Print(
        SyntaxToken openParenToken,
        SeparatedSyntaxList<ArgumentSyntax> arguments,
        SyntaxToken closeParenToken,
        FormattingContext context
    )
    {
        var docs = new List<Doc> { Token.Print(openParenToken, context) };

        if (arguments.Any())
        {
            docs.Add(
                Doc.Indent(
                    Doc.SoftLine,
                    SeparatedSyntaxList.Print(arguments, Argument.Print, Doc.Line, context)
                ),
                Doc.SoftLine
            );
        }

        docs.Add(Token.Print(closeParenToken, context));

        return Doc.Concat(docs);
    }
}
