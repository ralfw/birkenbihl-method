using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using Xunit;
using Xunit.Abstractions;

namespace DeepLTranslation
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var sut = new DeepLTranslator();
            var result = sut.Usage();
            _testOutputHelper.WriteLine($"::: {result.charCount} of {result.charLimit}");
        }
    }


    public class DeepLTranslator
    {
        // API documentation: https://www.deepl.com/docs-api/accessing-the-api/

        private string _authkey;
        private RestClient _client;
        
        public DeepLTranslator() {
            _authkey = File.ReadAllText("deeplauthkey.secret.txt");
            _client = new RestClient("https://api-free.deepl.com/v2/");
        }


        class UsageResponse
        {
            public int character_count { get; set; }
            public int character_limit { get; set; }
        }
        
        // API documentation: https://www.deepl.com/de/docs-api/accessing-the-api/
        public (int charCount, int charLimit) Usage() {
            var request = new RestRequest("usage");
            request.AddHeader("Authorization", $"DeepL-Auth-Key {_authkey}");

            var resp = _client.Get(request);

            var data = System.Text.Json.JsonSerializer.Deserialize<UsageResponse>(resp.Content);
            return (data.character_count, data.character_limit);
        }
        
        public string Translate(string original)
        {
            throw new NotImplementedException();
        }
    }
}