using System.IO;
using System.Net;
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
            var transcriptionId = sut.Convert("Така беше и тази година.", "bg-BG-KalinaNeural");
            var result = sut.WaitForTranscription(transcriptionId);
            _testOutputHelper.WriteLine($"::: {result.url} / {result.duration}");

            const string MP3_FILENAME = "transcription.mp3";
            if (File.Exists(MP3_FILENAME)) File.Delete(MP3_FILENAME);
            new WebClient().DownloadFile(result.url, MP3_FILENAME);
        }
        
        [Fact]
        public void Convert_and_wait_french()
        {
            var sut = new PlayhtTranscriber();
            var transcriptionId = sut.Convert("VOYAGE  AU  CENTRE DE  LA TERRE", "fr-FR-Standard-E");
            var result = sut.WaitForTranscription(transcriptionId);
            _testOutputHelper.WriteLine($"::: {result.url} / {result.duration}");

            const string MP3_FILENAME = "transcription.mp3";
            if (File.Exists(MP3_FILENAME)) File.Delete(MP3_FILENAME);
            new WebClient().DownloadFile(result.url, MP3_FILENAME);
        }
    }
}