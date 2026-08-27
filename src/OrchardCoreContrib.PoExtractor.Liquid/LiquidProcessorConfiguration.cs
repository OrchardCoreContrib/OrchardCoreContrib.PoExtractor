namespace OrchardCoreContrib.PoExtractor.Liquid;

public class LiquidProcessorConfiguration
{
    public IReadOnlyList<string> InlineTags { get; set; } = [];

    public IReadOnlyList<string> BlockTags { get; set; } = [];
}
