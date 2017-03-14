using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Yargon.Parsing
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            var lexer = new RegexLexer<TokenType>(new []
            {
                (TokenType.Zero, new Regex("0")),
                (TokenType.One, new Regex("1")),
            });

            // Act
            var tokens = (IEnumerable<Token<TokenType>>)lexer.Lex(new StringReader("010"));

            // Assert
            Assert.Equal(new [] { TokenType.Zero, TokenType.One, TokenType.Zero }, tokens.Select(t => t.Type));
        }

        public enum TokenType
        {
            Zero,
            One
        }
    }
}
