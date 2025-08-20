namespace OrchardCoreContrib.PoExtractor;

/// <summary>
/// Extracts a translatable string from a node of the AST tree.
/// </summary>
/// <typeparam name="TNode">Type of the node</typeparam>
public interface IStringExtractor<TNode>
{
    /// <summary>
    /// Tries to extract a localizable string from the AST node.
    /// </summary>
    /// <param name="node">The AST node.</param>
    /// <param name="result">The extracted localizable string.</param>
    /// <returns><c>true</c> if a localizable string was successfully extracted, otherwise returns <c>false</c>.</returns>
    bool TryExtract(TNode node, out LocalizableStringOccurence result);
}
