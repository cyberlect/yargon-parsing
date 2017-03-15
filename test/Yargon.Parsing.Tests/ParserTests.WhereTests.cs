using System;
using System.Linq;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Where"/> method.
        /// </summary>
        public sealed class WhereTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceed_WhenInputParserSucceedsAndPredicateSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.Where(t => true);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(TokenType.Zero, result.Value.Type);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenInputParserSucceedsButPredicateFails()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.Where(t => false);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<Token<TokenType>>();
                var parser = firstParser.Where(t => true);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Where(t => true);

                // Act
                var exception = Record.Exception(() =>
                {
                    parser(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenFirstParserIsNull()
            {
                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Where<String, Token<TokenType>>(null, t => true);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenPredicateFunctionIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    firstParser.Where<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
