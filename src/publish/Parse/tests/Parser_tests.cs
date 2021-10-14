using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Parse
{
    public class Parser_tests
    {
        [Fact]
        public void Parse_paragraphs()
        {
            var result = Parser.Parse(@"1.1
1.2

2.1
2.2
2.3

3.1");

            result.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(new[]
            {
                @"1.1
1.2",
                @"2.1
2.2
2.3",
                @"3.1"
            });
        }
        
        
        [Fact]
        public void Parse_paragraphs_with_leading_trailing_blank_lines()
        {
            var result = Parser.Parse(@"

1.1
1.2

2.1
2.2
2.3


");

            result.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(new[]
            {
                @"1.1
1.2",
                @"2.1
2.2
2.3"
            });
        }
        
        
        [Fact]
        public void Parse_paragraphs_with_more_than_one_blank_line_between_paragraphs()
        {
            var result = Parser.Parse(@"1.1
1.2




2.1
2.2
2.3");

            result.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(new[]
            {
                @"1.1
1.2",
                @"2.1
2.2
2.3"
            });
        }
        
        
        [Fact]
        public void Parse_paragraphs_with_comment_lines()
        {
            var result = Parser.Parse(@"
// comment
1.1
// comment
1.2
// comment

2.1
2.2
// comment
// comment
2.3
// comment");

            result.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(new[]
            {
                @"1.1
1.2",
                @"2.1
2.2
2.3"
            });
        }


        [Fact]
        public void Detect_headline_levels() {
            var result = Parser.Parse("0.1\n\n# 1 \n\n##  2\n\n### 3a\n3b\n\n0.2");
            result.Paragraphs.Select(p => p.HeadlineLevel).Should().BeEquivalentTo(new[] {0, 1, 2, 3, 0});
            result.Paragraphs.Select(p => p.Text).Should().BeEquivalentTo(new[] {"0.1", "1 ", "2", "3a\n3b", "0.2"});
        }
        
        

        [Fact]
        public void Parse_chunks()
        {
            var result = Parser.Parse(" \"абц\"    ьзъ.\nяве-ртъ! ");
            result.Paragraphs[0].Chunks.Select(ch => ch.Text).Should().BeEquivalentTo(new[] {"\"абц\"", "ьзъ.", "яве-ртъ!"});
        }
        
        [Fact]
        public void Parse_chunks_separated_by_tabs()
        {
            var result = Parser.Parse("a\tbb. \tccc");
            result.Paragraphs[0].Chunks.Select(ch => ch.Text).Should().BeEquivalentTo(new[] {"a", "bb.", "ccc"});
        }
        
        [Fact]
        public void Parse_words() {
            var result = Parser.Parse(" \"абц\"  +123()  ьзъ.\nяве-ртъ!xyz ");
            result.Paragraphs[0].Chunks.Select(ch => ch.Word).Should().BeEquivalentTo(new[] {"абц", "", "ьзъ", "яве-ртъ"});
        }
        
        [Fact]
        public void Parse_words_with_spaces() {
            var result = Parser.Parse("a_b");
            result.Paragraphs[0].Chunks.Select(ch => ch.Word).Should().BeEquivalentTo(new[] {"a b"});
        }



        [Fact]
        public void Index() {
            var result = Parser.Parse("a d-ef +bc def bc. 123");
            result.Index.Should().BeEquivalentTo(new[] {"a", "bc", "def", "d-ef"});
        }
    }
}