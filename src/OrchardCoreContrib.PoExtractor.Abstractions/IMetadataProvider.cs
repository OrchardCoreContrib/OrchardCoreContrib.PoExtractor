namespace OrchardCoreContrib.PoExtractor
{
    /// <summary>
    /// Provides metadata of the translatable text based on information from the AST node.
    /// </summary>
    /// <typeparam name="TNode">Type of the node.</typeparam>
    public interface IMetadataProvider<TNode>
    {
        /// <summary>
        /// Gets context of the translatable text.
        /// </summary>
        /// <param name="node">The AST node representing the translatable text.</param>
        /// <returns>A string value, that is used in the output file as #msgctx.</returns>
        string GetContext(TNode node);

        /// <summary>
        /// Gets location of the translatable text in the source file.
        /// </summary>
        /// <param name="node">The AST node representing the translatable text.</param>
        /// <returns>An object with the description of the location in the source file.</returns>
        LocalizableStringLocation GetLocation(TNode node);
    }
}
