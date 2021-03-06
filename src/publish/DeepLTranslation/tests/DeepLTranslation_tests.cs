using System;
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
    public class DeepLTranslation_tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DeepLTranslation_tests(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Get_usage()
        {
            var sut = new DeepLTranslator();
            var result = sut.Usage();
            _testOutputHelper.WriteLine($"::: {result.charCount} ({result.charCount/result.charLimit:P}) of {result.charLimit}");
        }
        
        [Fact]
        public void Translate_paragraph()
        {
            var sut = new DeepLTranslator();
            var result = sut.Translate(@"Така беше и тази година. Дядо Коледа непрекъснато обикаляше фабриката.

            Трябваше да е сигурен, че всичко е наред.", "BG", "EN-US");
            _testOutputHelper.WriteLine($"::: {result}");
        }
        
        [Fact]
        public void Translate_words()
        {
            var sut = new DeepLTranslator();
            var result = sut.Translate(new[]
                {"Така", "беше", "и", "тази", "година", 
                    "Дядо Коледа", "непрекъснато", "обикаляше", "фабриката"}, "BG", "EN-US");
            foreach(var r in result)
                _testOutputHelper.WriteLine($"::: {r}");
        }
        
        
        [Fact]
        public void Translate_french()
        {
            var sut = new DeepLTranslator();
            var result = sut.Translate("VOYAGE  AU  CENTRE DE  LA TERRE", "FR", "EN-US");
            _testOutputHelper.WriteLine($"::: {result}");
        }
        
        
        [Fact]
        public void Translate_non_existing_word()
        {
            var sut = new DeepLTranslator();
            var result = sut.Translate("XYZ", "FR", "EN-US");
            _testOutputHelper.WriteLine($"::: {result}");
        }
    }
}