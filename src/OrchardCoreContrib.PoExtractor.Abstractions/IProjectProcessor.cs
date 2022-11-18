namespace OrchardCoreContrib.PoExtractor
{
    public interface IProjectProcessor
    {
        void Process(string path, string basePath, LocalizableStringCollection strings);
    }
}
