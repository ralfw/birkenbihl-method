using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RestSharp;

namespace DeepLTranslation
{
    public class DeepLTranslator
    {
        // API documentation: https://www.deepl.com/docs-api/accessing-the-api/

        private string _authkey;
        private RestClient _client;
        
        public DeepLTranslator() {
            _authkey = File.ReadAllText("deeplauthkey.secret.txt");
            _client = new RestClient("https://api-free.deepl.com/v2/");
        }



        // API documentation: https://www.deepl.com/de/docs-api/accessing-the-api/
        private record UsageResponse(int character_count, int character_limit) { }
        
        public (int charCount, int charLimit) Usage() {
            var request = new RestRequest("usage");
            request.AddHeader("Authorization", $"DeepL-Auth-Key {_authkey}");
            var resp = _client.Get(request);
            
            var data = System.Text.Json.JsonSerializer.Deserialize<UsageResponse>(resp.Content);
            return (data.character_count, data.character_limit);
        }
        
        
        // API documentation: https://www.deepl.com/de/docs-api/translating-text/request/
        private record Translation(string detected_source_language, string text) {}
        private record TranslationResponse(Translation[] translations) { }

        public string Translate(string original) => Translate(new[] {original})[0];
        
        public string[] Translate(IEnumerable<string> originals)
        {
            const string SOURCE_LANG = "BG";
            const string TARGET_LANG = "EN-US";
            
            var request = new RestRequest($"translate");
            request.AddParameter("auth_key", _authkey);
            request.AddParameter("source_lang", SOURCE_LANG);
            request.AddParameter("target_lang", TARGET_LANG);
            //request.AddParameter("split_sentences", "nonewlines");
            foreach(var o in originals)
                request.AddParameter("text", o);
            var resp = _client.Post(request);
            
            var data = System.Text.Json.JsonSerializer.Deserialize<TranslationResponse>(resp.Content);
            return data.translations.Select(x => x.text).ToArray();
        }
    }
}