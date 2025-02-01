using EntwineBackend.DbItems;

namespace EntwineBackend_Tests
{
    internal static class TestConstants
    {
        public static readonly InputLocation TestLocation = new()
        {
            City = "TestCity",
            Country = "TestCountry",
            State = "TestState"
        };

        public static readonly InputLocation TestLocation2 = new()
        {
            City = "TestCity2",
            Country = "TestCountry2",
            State = "TestState2"
        };
    }
}
