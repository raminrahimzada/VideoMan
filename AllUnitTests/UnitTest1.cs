using VideoMan;
using Xunit;

namespace AllUnitTests
{
    public class UnitTest1
    {
        
        [Fact]
        public void TestDependencyInjection()
        {
            App.RegisterStartup();
            Assert.Equal(App.Client.GetType().FullName, typeof(SimpleWebClient).FullName);
        }
        [Fact]
        public void TestStringHelper1()
        {
            const int max = 10;
            for (int i = 0; i <max; i++)
            {
                var part1 = RandomHelpers.RandomString(100);
                var part2 = RandomHelpers.RandomString(100);
                var part3 = RandomHelpers.RandomString(100);
                var text = part1 + part2 + part3;
                var middle = text.GetStringBetween(part1, part3);
                Assert.Equal(part2, middle);
            }
         
            //extra case
            Assert.Equal("-test-asdfg-", "blabla-XXXXX-test-asdfg-xyz".GetStringBetween("XXXXX", "xyz"));
        }
    }
}
