using Microsoft.CodeAnalysis;

namespace PoExtractor.Core.Contracts {
    /// <summary>
    /// Provides metadata of the translatable text based on information from the AST node
    /// </summary>
    /// <typeparam name="T">type of the node</typeparam>
    public interface IMetadataProvider<T> {
        /// <summary>
        /// Gets context of the translatable text
        /// </summary>
        /// <param name="node">the AST node representing the translatable text</param>
        /// <returns>a string value, that is used in the output file as #msgctx</returns>
        string GetContext(T node);

        /// <summary>
        /// Gets location of the translatable text in the source file
        /// </summary>
        /// <param name="node">the AST node representing the translatable text</param>
        /// <returns>an object with the description of the location in the source file</returns>
        LocalizableStringLocation GetLocation(T node);
    }
}
