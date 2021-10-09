using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Parse
{
    public class Text {
        public class Paragraph {
            public class Chunk {
                internal Chunk(string text) => Text = text;
                
                public string Text { get; init; }

                public string Word {
                    get {
                        // skip non-word characters
                        var chars = new Queue<char>(Text.ToCharArray());
                        while (chars.Count() > 0 &&  IsWordCharacter(chars.Peek()) is false)
                            chars.Dequeue();

                        // compile word characters
                        var text = "";
                        while (chars.Count > 0 && IsWordCharacter(chars.Peek()))
                            text += chars.Dequeue();

                        return text.Replace("_", " ");

                        bool IsWordCharacter(char c) => char.IsLetter(c) || c == '-' || c == '–' || c == '_';
                    }
                }
            }


            internal Paragraph(string text) {
                var i = 0;
                while (i < text.Length && text[i] == '#')
                    i++;
                HeadlineLevel = i;

                if (HeadlineLevel > 0) text = text.Substring(HeadlineLevel).TrimStart();
                Text = text;
            }
            
            
            public int HeadlineLevel { get; private set; }
            
            public string Text { get; init; }
            
            public Chunk[] Chunks {
                get {
                    return this.Text.Split(new[] {" ", "\n", "\r"},
                            StringSplitOptions.TrimEntries | 
                            StringSplitOptions.RemoveEmptyEntries)
                        .Select(ch => new Chunk(ch)).ToArray();
                }
            }
        }
        
        
        internal Text(IEnumerable<Paragraph> paragraphs) => Paragraphs = paragraphs.ToArray();
        
        public Paragraph[] Paragraphs { get; init; }

        public string[] Index
        {
            get {
                return Paragraphs.SelectMany(p => p.Chunks.Select(ch => ch.Word))
                    .Where(w => !string.IsNullOrWhiteSpace(w))
                    .Distinct()
                    .OrderBy(w => w)
                    .ToArray();
            }
        }
    }
}