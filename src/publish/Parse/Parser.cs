using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parse
{
    public static class Parser
    {
        public static IEnumerable<string> ParseIntoParagraphs(string text) {
            var sr = new StringReader(text);
            var p = new StringBuilder();
            do {
                var line = sr.ReadLine();
                if (line == null) break;

                if (line.Trim().StartsWith("//")) // skip comment lines
                    continue;
                
                if (string.IsNullOrWhiteSpace(line)) {
                    if (p.Length > 0)
                        yield return p.ToString();
                    p.Clear();
                }
                else {
                    if (p.Length > 0)
                        p.AppendLine("");
                    p.Append(line);
                }
            } while (true);
            if (p.Length > 0)
                yield return p.ToString();
        }

        
        public static IEnumerable<string> ParseIntoWords(string text) {
            var w = "";
            foreach (var c in text) {
                if (char.IsLetter(c) || c == '-' || c == '–')
                    w += c;
                else {
                    if (w.Length > 0) {
                        yield return w;
                        w = "";
                    }
                }
            }
            if (w.Length > 0) yield return w;
        }
    }
}