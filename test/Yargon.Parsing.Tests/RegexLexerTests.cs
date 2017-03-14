using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Yargon.Parsing
{
    /// <summary>
    /// Tests the <see cref="RegexLexer{TTokenType}"/> class.
    /// </summary>
    public class RegexLexerTests
    {
        [Fact]
        public void SimpleExample_ShouldParseToExpectedTokens()
        {
            // Arrange
            var lexer = new RegexLexer<TokenType>(new Dictionary<string, TokenType>
            {
                ["0"] = TokenType.Zero,
                ["1"] = TokenType.One,
            });

            // Act
            var tokens = (IEnumerable<Token<TokenType>>)lexer.Lex(new StringReader("010"));

            // Assert
            Assert.Equal(new []
            {
                ("0", TokenType.Zero),
                ("1", TokenType.One),
                ("0", TokenType.Zero)
            }, tokens.Select(t => (t.Value, t.Type)));
        }

        public enum TokenType
        {
            Zero,
            One
        }
    }
}
