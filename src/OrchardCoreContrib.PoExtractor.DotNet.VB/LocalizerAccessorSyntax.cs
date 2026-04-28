using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace OrchardCoreContrib.PoExtractor.DotNet.VB;

internal static class LocalizerAccessorSyntax
{
    public static bool IsLocalizerAccessor(ExpressionSyntax expression) =>
        GetLocalizerIdentifier(expression) is { } identifier &&
        LocalizerAccessors.IsLocalizerIdentifier(identifier);

    private static string GetLocalizerIdentifier(ExpressionSyntax expression) => expression switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.Text,
        GenericNameSyntax genericName => genericName.Identifier.Text,
        _ => null
    };
}
