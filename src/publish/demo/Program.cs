using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using DeepLTranslation;
using GoogleTranslation;
using Parse;
using PlayhtTranscription;

namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Приказки за Дядо Коледа.md
            var birkenbihl = new BirkenbihlMethod(new DeepLTranslator(), new GoogleTranslator(), new PlayhtTranscriber());
            birkenbihl.Compile("test.md", "./test");
        }
    }


    public class BirkenbihlMethod
    {
        private readonly DeepLTranslator _paragraphTranslator;
        private readonly GoogleTranslator _wordTranslator;
        private readonly PlayhtTranscriber _transcriber;

        public BirkenbihlMethod(DeepLTranslator paragraphTranslator, GoogleTranslator wordTranslator, PlayhtTranscriber transcriber)
        {
            _paragraphTranslator = paragraphTranslator;
            _wordTranslator = wordTranslator;
            _transcriber = transcriber;
        }


        public void Compile(string mdSourceFilepath, string targetFolderpath)
        {
            // languages supported: https://www.deepl.com/de/docs-api/translating-text/request/
            const string bgDeepL = "BG";
            const string frDeepL = "FR";
            const string enDeepL = "EN-US";
            const string ltDeepL = "LT";
            const string deDeepL = "DE";
            
            // languages supported: https://cloud.google.com/translate/docs/languages
            const string bgGoogle = "bg";
            const string frGoogle = "fr";
            const string enGoogle = "en";
            const string ltGoogle = "lt";
            const string deGoogle = "de";
            
            // voice reference file: https://github.com/playht/text-to-speech-api/blob/master/Voices.md
            const string bgPlayht = "bg-BG-KalinaNeural";
            const string frPlayht = "fr-FR-Standard-E";
            const string ltPlayht = "lt-LT-OnaNeural";

            // source lang
            var slDeepL = ltDeepL;
            var slGoogle = ltGoogle;
            var slPlayht = ltPlayht;
            
            // target lang
            var tlDeepL = enDeepL;
            var tlGoogle = enGoogle;
            
            
            // publication could be done on vercel oder durch eine github page.
            // create folder with...
            //      index.html
            //      *.mp3
            if (Directory.Exists(targetFolderpath)) Directory.Delete(targetFolderpath, true);
            Directory.CreateDirectory(targetFolderpath);
            
            // get paragraphs
            Console.WriteLine("Loading...");
            var text = Parser.Parse(File.ReadAllText(mdSourceFilepath));
            Console.WriteLine($"{text.Paragraphs.Length} paragraphs / {text.Paragraphs.Select(p => p.Chunks.Length).Sum()} words");

            
            // translate paragraphs
            Console.WriteLine("Translating parapgraphs...");
            var paragraphTranslations = new List<string>();
            foreach (var paragraph in text.Paragraphs) {
                Console.WriteLine($"    {paragraph.Text.Substring(0, Math.Min(10, paragraph.Text.Length))}...");
                var translation = _paragraphTranslator.Translate(paragraph.Text, slDeepL, tlDeepL);
                paragraphTranslations.Add(translation);
            }
            
            // translate index (DeepL: could be chunked with 50 words at a time)
            Console.WriteLine($"Translating index of {text.Index.Length} words...");
            var dictionary = new Dictionary<string, string>();
            foreach (var word in text.Index) {
                Console.Write(".");
                var translation = _wordTranslator.Translate(word, slGoogle, tlGoogle);
                dictionary.Add(word, translation);
            }
            Console.WriteLine();
            
            // transcribe paragraphs
            Console.WriteLine("Transcribing...");
            for(var p=0; p < text.Paragraphs.Length; p++)
            {
                var txid = _transcriber.Convert(text.Paragraphs[p].Text, slPlayht);
                var tsc = _transcriber.WaitForTranscription(txid);

                var mp3Filename = Path.Combine(targetFolderpath, $"p{p:000}.mp3");
                if (File.Exists(mp3Filename)) File.Delete(mp3Filename);
                new WebClient().DownloadFile(tsc.url, mp3Filename);
                Console.Write(".");
            }
            Console.WriteLine();

            // render a Birkenbihl text...
            // color names: https://htmlcolorcodes.com/
            Console.WriteLine("Rendering html...");
            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            for(var i=0; i<text.Paragraphs.Length; i++) {
                // paragraph
                // original
                var paragraph = text.Paragraphs[i];
                string[] FONT_SIZES = new[] {"100%", "200%", "160%", "130%", "110%", "110%", "110%", "110%", "110%"};
                html.AppendLine($"<div style=\"background-color:PapayaWhip;font-size:{FONT_SIZES[paragraph.HeadlineLevel]}\">{paragraph.Text}&nbsp🔈<span id='play{i:000}' style='cursor:pointer' onclick=\"toggle('{i:000}')\">▶️</span></div>");
                // audio player
                html.AppendLine($"<audio controls id='audio{i:000}' style='display:none'><source src='p{i:000}.mp3'/></audio>");
                // translated
                html.AppendLine($"<div style=\"color:gray;background-color:PapayaWhip;\">{paragraphTranslations[i]}</div>");

                // word by word interlinear paragraph
                // lines max 80chars wide
                // min space between words: 1 char
                // original: chunk from paragraph
                // translation: translation from index for word from chunk
                var pairs = paragraph.Chunks.Select(ch => new {
                    chunk = ch.Text,
                    translation = string.IsNullOrWhiteSpace(ch.Word) ? ch.Text : dictionary[ch.Word]
                }).ToArray();

                html.AppendLine("<table style=\"border:1px gray;border-collapse:collapse;border-style:dotted\">");
                const int MAX_LINE_LEN = 80;
                var j = 0;
                var originalLineChunks = new List<string>();
                var translatedLineWords = new List<string>();
                while (j < pairs.Length) {
                    if (FitsInLine(originalLineChunks, pairs[j].chunk, MAX_LINE_LEN) &&
                        FitsInLine(translatedLineWords, pairs[j].translation, MAX_LINE_LEN)) {
                        originalLineChunks.Add(pairs[j].chunk);
                        translatedLineWords.Add(pairs[j].translation);
                        j++;
                    }
                    else {
                        RenderInterlinear();

                        originalLineChunks = new List<string>();
                        translatedLineWords = new List<string>();
                    }
                }
                RenderInterlinear();
                html.AppendLine("</table>");

                
                bool FitsInLine(List<string> line, string word, int maxLineLen)
                    => line.Sum(w => w.Length) + line.Count() + word.Length <= maxLineLen;

                void RenderInterlinear() {
                    if (originalLineChunks.Count() == 0) return;
                    
                    html.AppendLine("<tr><td><table>");

                    var originalLine = "<tr style=\"background-color:#9FE2BF;\">";
                    var translatedLine = "<tr style=\"color:gray;background-color:LightYellow;\">";
                    for (var k = 0; k < originalLineChunks.Count(); k++) {
                        originalLine += $"<td>{originalLineChunks[k]}</td>";
                        translatedLine += $"<td>{translatedLineWords[k]}</td>";
                    }
                    originalLine += "</tr>";
                    translatedLine += "</tr>";
                    
                    html.AppendLine(originalLine);
                    html.AppendLine(translatedLine);
                    
                    html.AppendLine("</td></tr></table>");

                }
            }

            // Documentation: https://www.w3schools.com/howto/howto_js_toggle_hide_show.asp
            html.AppendLine(@"
<script>
function toggle(soundnumber) {
    var audio = document.getElementById('audio'+soundnumber);
    var btn = document.getElementById('play'+soundnumber);

    var isPlaying = btn.innerText == '⏹';
    if (isPlaying) {
        audio.pause();
        audio.currentTime = 0; // stops by rewinding the audio
        btn.innerText = '▶️';
    } else {
        btn.innerText = '⏹';
        audio.onended = function() {
            btn.innerText = '▶️';
        };
        audio.play();
    }
}
</script>");
            html.AppendLine("</body></html>");
            File.WriteAllText(Path.Combine(targetFolderpath, "index.html"), html.ToString());
            Console.WriteLine("Done!");
        }
    }
}