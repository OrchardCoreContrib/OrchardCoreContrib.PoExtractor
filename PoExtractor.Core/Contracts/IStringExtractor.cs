using Microsoft.CodeAnalysis;

namespace PoExtractor.Core.Contracts {
    /// <summary>
    /// Extracts a translatable string from a node of the AST tree
    /// </summary>
    /// <typeparam name="T">type of the node</typeparam>
    public interface IStringExtractor<T> {
        /// <summary>
        /// Tries to extract a localizable string from the AST node
        /// </summary>
        /// <param name="node">the AST node</param>
        /// <param name="result">the extracted localizable string</param>
        /// <returns>true if a localizable string was successfully extracted, otherwise returns false</returns>
        bool TryExtract(T node, out LocalizableStringOccurence result);
    }
}
