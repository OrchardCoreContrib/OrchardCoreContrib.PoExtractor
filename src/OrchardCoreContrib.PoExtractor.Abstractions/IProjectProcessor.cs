namespace OrchardCoreContrib.PoExtractor;

/// <summary>
/// Contract for processing a project to get the localization strings.
/// </summary>
public interface IProjectProcessor
{
    /// <summary>
    /// Lookup for the localizable string by process the given project path.
    /// </summary>
    /// <param name="path">Project path.</param>
    /// <param name="basePath">Project base path.</param>
    /// <param name="localizableStrings">List of <see cref="LocalizableString"/> contain in the processed project.</param>
    void Process(string path, string basePath, LocalizableStringCollection localizableStrings);
}
