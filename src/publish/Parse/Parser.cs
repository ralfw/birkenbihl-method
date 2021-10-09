using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Parse
{
    public static class Parser
    {
        public static Text Parse(string text)
            => new Text(ParseIntoParagraphs(text).Select(p => new Text.Paragraph(p)));
        
        
        private static IEnumerable<string> ParseIntoParagraphs(string text) {
            var sr = new StringReader(text);
            var p = new StringBuilder();
            do {
                var line = sr.ReadLine();
                if (line == null) break;

                // skip comment lines
                if (line.Trim().StartsWith("//"))
                    continue;
                
                // detect end-of-paragraph
                if (string.IsNullOrWhiteSpace(line)) {
                    if (p.Length > 0)
                        yield return p.ToString();
                    p.Clear();
                }
                // append line to paragraph
                else {
                    if (p.Length > 0)
                        p.AppendLine("");
                    p.Append(line);
                }
            } while (true);
            if (p.Length > 0)
                yield return p.ToString();
        }
    }
}