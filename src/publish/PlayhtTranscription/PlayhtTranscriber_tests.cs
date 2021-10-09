using System;
using System.IO;
using System.Net;
using RestSharp;
using Xunit;
using Xunit.Abstractions;

namespace PlayhtTranscription
{
    public class PlayhtTranscriber_tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PlayhtTranscriber_tests(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        
        [Fact]
        public void Convert_and_wait()
        {
            var sut = new PlayhtTranscriber();
            var transcriptionId = sut.Convert("Така беше и тази година.");
            var result = sut.WaitForTranscription(transcriptionId);
            _testOutputHelper.WriteLine($"::: {result.url} / {result.duration}");

            const string MP3_FILENAME = "transcription.mp3";
            if (File.Exists(MP3_FILENAME)) File.Delete(MP3_FILENAME);
            new WebClient().DownloadFile(result.url, MP3_FILENAME);
        }
    }



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
        
        public string Convert(string text) {
            const string VOICE = "bg-BG-KalinaNeural";
            
            var request = new RestRequest("convert", DataFormat.Json);
            request.AddJsonBody(new ConvertRequest(VOICE, new[] {text}, new string[0], "66%"));
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