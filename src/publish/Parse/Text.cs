using System;
using System.Collections.Generic;
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

                        return text;

                        bool IsWordCharacter(char c) => char.IsLetter(c) || c == '-' || c == '–';
                    }
                }
            }


            internal Paragraph(string text) => Text = text;
            
            
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
    }
}