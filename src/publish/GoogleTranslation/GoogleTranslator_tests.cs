using System.Collections;
using System.Net.Http;
using Xunit;
using RestSharp.Serialization.Json;
using Xunit.Abstractions;

namespace GoogleTranslation
{
    public class GoogleTranslator_tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GoogleTranslator_tests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Translate()
        {
            var sut = new GoogleTranslator();
            
            var result = sut.Translate("Трябваше да е сигурен, че всичко е наред.", "bg", "en");
            _testOutputHelper.WriteLine(":::" + result);
            
            result = sut.Translate("и", "bg", "en");
            _testOutputHelper.WriteLine(":::" + result);
            
            result = sut.Translate("е", "bg", "en");
            _testOutputHelper.WriteLine(":::" + result);
            
            result = sut.Translate("VOYAGE  AU  CENTRE DE  LA TERRE", "fr", "en");
            _testOutputHelper.WriteLine(":::" + result);
        }
    }
}