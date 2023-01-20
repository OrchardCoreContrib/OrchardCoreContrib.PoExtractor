namespace OrchardCoreContrib.PoExtractor.Tests.Fakes
{
    public class FakeStringLocalizer
    {
        public string this[string singular]
        {
            get => singular;
        }

        public void Plural(int count, string singular, string plural, params object[] arguments)
        {
        }
    }
}
