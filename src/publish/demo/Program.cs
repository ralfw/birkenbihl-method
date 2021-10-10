using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DeepLTranslation;
using Parse;
using PlayhtTranscription;

namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Приказки за Дядо Коледа.md
            var birkenbihl = new BirkenbihlMethod(new DeepLTranslator(), null);
            birkenbihl.Compile("test.md", "./test");
        }
    }


    public class BirkenbihlMethod
    {
        private readonly DeepLTranslator _translator;

        public BirkenbihlMethod(DeepLTranslator translator, PlayhtTranscriber transcriber) {
            _translator = translator;
        }


        public void Compile(string mdSourceFilepath, string targetFolderpath)
        {
            // publication could be done on vercel oder durch eine github page.
            // create folder with...
            //      text.html
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
                var translation = _translator.Translate(paragraph.Text);
                paragraphTranslations.Add(translation);
            }
            
            // translate index (could be chunked with 50 words at a time)
            Console.WriteLine($"Translating index of {text.Index.Length} words...");
            var dictionary = new Dictionary<string, string>();
            foreach (var word in text.Index) {
                Console.Write(".");
                var translation = _translator.Translate(word);
                dictionary.Add(word, translation);
            }
            Console.WriteLine();

            Console.WriteLine("Rendering html...");
            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            for(var i=0; i<text.Paragraphs.Length; i++) {
                // paragraph
                // original
                var paragraph = text.Paragraphs[i];
                if (paragraph.HeadlineLevel > 0) {
                    html.AppendLine($"<h{paragraph.HeadlineLevel}>{paragraph.Text}</h{paragraph.HeadlineLevel}>");
                }
                else {
                    html.AppendLine($"<p>{paragraph.Text}</p>");
                }
                // translated
                // color names: https://www.w3schools.com/tags/ref_colornames.asp
                html.AppendLine($"<p style=\"color:gray;\">{paragraphTranslations[i]}</p>");
                
                // word by word interlinear paragraph
                // monospaced font drumherum wickeln
                // lines max 80chars wide
                // min space between words: 1 char
                // original: chunk from paragraph
                // translation: translation from index for word from chunk
                var pairs = paragraph.Chunks.Select(ch => new {
                    chunk = ch.Text,
                    translation = dictionary[ch.Word]
                }).ToArray();

                html.AppendLine("<p style=\"font-family:courier;font-size:80%\">");
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
                html.AppendLine("</p>");

                
                bool FitsInLine(List<string> line, string word, int maxLineLen)
                    => line.Sum(w => w.Length) + line.Count() + word.Length <= maxLineLen;

                void RenderInterlinear() {
                    if (originalLineChunks.Count() == 0) return;
                    
                    var originalLine = "";
                    var translatedLine = "";
                    for (var k = 0; k < originalLineChunks.Count(); k++) {
                        var width = Math.Max(originalLineChunks[k].Length, translatedLineWords[k].Length);
                        if (originalLine.Length > 0) {
                            originalLine += " ";
                            translatedLine += " ";
                        }
                        originalLine += originalLineChunks[k].PadRight(width);
                        translatedLine += translatedLineWords[k].PadRight(width);
                    }
                    html.AppendLine($"<span>{originalLine}</span><br/>");
                    html.AppendLine($"<span>{translatedLine}</span><br/>");
                }
            }
            html.AppendLine("</body></html>");
            File.WriteAllText(Path.Combine(targetFolderpath, "text.html"), html.ToString());
            Console.WriteLine("Done!");
        }
    }
}