using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using RestSharp;

namespace GoogleTranslation
{
    public class GoogleTranslator
    {
        // API documentation: https://www.codeproject.com/Tips/5247661/Google-Translate-API-Usage-in-Csharp

        private RestClient _client;
        
        public GoogleTranslator() {
            _client = new RestClient("https://translate.googleapis.com/translate_a/");
        }


        // bg, en, fr
        public string Translate(string original, string sourceLanguage, string targetLanguage)
        {
            var request = new RestRequest($"single?client=dict-chrome-ex&sl={sourceLanguage}&tl={targetLanguage}&dt=t&q=" + Uri.EscapeUriString(original));
            // Add User-Agent header to avoid Captcha request.
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.104 Safari/537.36");
            var resp = _client.Get(request);
            
            /*
                Json structure:
                    [
                      [
                        ["Haus","house",null,null,1]
                      ],
                      null,"en",null,null,null,null,[]
                    ]
             */
            var data = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(resp.Content);
            return ((JsonElement)data[0]).EnumerateArray().First().EnumerateArray().First().GetString();
        }
    }
}