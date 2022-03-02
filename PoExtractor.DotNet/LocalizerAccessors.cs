namespace PoExtractor.DotNet
{
    public static class LocalizerAccessors
    {
        public static readonly string DefaultLocalizerIdentifier = "T";

        public static readonly string DefaultLocalizerPrivateIdentifier = "_t";

        public static readonly string StringLocalizerIdentifier = "S";

        public static readonly string StringLocalizerPrivateIdentifier = "_s";

        public static readonly string HtmlLocalizerIdentifier = "H";

        public static readonly string HtmlLocalizerPrivateIdentifier = "_h";

        public static string[] LocalizerIdentifiers = new string[] {    DefaultLocalizerIdentifier, 
                                                                        DefaultLocalizerPrivateIdentifier, 
                                                                        StringLocalizerIdentifier, 
                                                                        StringLocalizerPrivateIdentifier, 
                                                                        HtmlLocalizerIdentifier, 
                                                                        HtmlLocalizerPrivateIdentifier 
                                                                    };
    }
}
..\