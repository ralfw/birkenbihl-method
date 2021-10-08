using System;
using FluentAssertions;
using Xunit;

namespace Parse
{
    public class Parser_tests
    {
        [Fact]
        public void Parse_paragraphs()
        {
            var result = Parser.ParseIntoParagraphs(@"1.1
1.2

2.1
2.2
2.3

3.1");

            result.Should().BeEquivalentTo(new[]
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
            var result = Parser.ParseIntoParagraphs(@"

1.1
1.2

2.1
2.2
2.3


");

            result.Should().BeEquivalentTo(new[]
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
            var result = Parser.ParseIntoParagraphs(@"1.1
1.2




2.1
2.2
2.3");

            result.Should().BeEquivalentTo(new[]
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
            var result = Parser.ParseIntoParagraphs(@"
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

            result.Should().BeEquivalentTo(new[]
            {
                @"1.1
1.2",
                @"2.1
2.2
2.3"
            });
        }
    }
}