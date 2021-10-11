using System;
using System.IO;
using RestSharp;

namespace PlayhtTranscription
{
    public record Transcription(string url, float duration);
    
    public class PlayhtTranscriber {
        // API documentation: https://github.com/playht/text-to-speech-api

        private RestClient _client;
        
        public PlayhtTranscriber() {
            var credentials = File.ReadAllLines("playht.secret.txt");
            _client = new RestClient("https://play.ht/api/v1/");
            _client.AddDefaultHeader("X-User-ID", credentials[0]);
            _client.AddDefaultHeader("Authorization", credentials[1]);
        }
        
        
        private record ConvertRequest(string voice, string[] content, string[] ssml, string globalSpeed){}
        private record ConvertResponse(string status, string transcriptionId, string error) { }
        
        // bg-BG-KalinaNeural, fr-FR-Standard-E
        public string Convert(string text, string voice) {
            var request = new RestRequest("convert", DataFormat.Json);
            request.AddJsonBody(new ConvertRequest(voice, new[] {text}, new string[0], "66%"));
            var resp = _client.Post(request);
            
            var data = System.Text.Json.JsonSerializer.Deserialize<ConvertResponse>(resp.Content);
            return data.transcriptionId;
        }


        private record ArticleStatus(bool converted, bool error, string errorMessage, string audioUrl, float audioDuration, string voice, string narrationStyle, string globalSpeed){}
        public bool IsTranscriptionComplete(string transcriptionId, out Transcription transcription) {
            var request = new RestRequest($"articleStatus?transcriptionId={transcriptionId}");
            var resp = _client.Get(request);
            
            var data = System.Text.Json.JsonSerializer.Deserialize<ArticleStatus>(resp.Content);

            transcription = null;
            if (data.converted) {
                transcription = new Transcription(data.audioUrl, data.audioDuration);
                return true;
            }

            if (data.error)
                throw new ApplicationException($"Transcription failed! {data.errorMessage}");
            else
                return false;
        }


        public Transcription WaitForTranscription(string transcriptionId) {
            Transcription transcription;
            while(IsTranscriptionComplete(transcriptionId, out transcription) is false) 
            {}
            return transcription;
        }
    }
}