using System;
using System.Collections.Generic;
using System.IO;
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
            // publication could be done on vercel!!!
            // create folder with...
            //      text.html
            //      *.mp3
            if (Directory.Exists(targetFolderpath)) Directory.Delete(targetFolderpath, true);
            Directory.CreateDirectory(targetFolderpath);
            
            var text = Parser.Parse(File.ReadAllText(mdSourceFilepath));

            Console.WriteLine("Translating parapgraphs...");
            var paragraphTranslations = new List<string>();
            foreach (var paragraph in text.Paragraphs) {
                Console.WriteLine($"    {paragraph.Text.Substring(0, Math.Min(10, paragraph.Text.Length))}...");
                var translation = _translator.Translate(paragraph.Text);
                paragraphTranslations.Add(translation);
            }
            

            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            for(var i=0; i<text.Paragraphs.Length; i++) {
                var paragraph = text.Paragraphs[i];
                if (paragraph.HeadlineLevel > 0) {
                    html.AppendLine($"<h{paragraph.HeadlineLevel}>{paragraph.Text}</h{paragraph.HeadlineLevel}>");
                }
                else {
                    html.AppendLine($"<p>{paragraph.Text}</p>");
                }
                // color names: https://www.w3schools.com/tags/ref_colornames.asp
                html.AppendLine($"<p style=\"color:gray;\">{paragraphTranslations[i]}</p>");
            }
            html.AppendLine("</body></html>");
            File.WriteAllText(Path.Combine(targetFolderpath, "text.html"), html.ToString());
        }
    }
}