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
        /// Tests the <see cref="Parser.Many"/> method.
        /// </summary>
        public sealed class ManyTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.Many();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { TokenType.Zero }, result.Value.Select(t => t.Type));
            }

            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnEmptySequence_WhenInputParserFailsImmediately()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var parser = firstParser.Many();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Value);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnConcatenatedInputParserMessages_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.Many();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "" }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Many();

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
                    Parser.Many<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
