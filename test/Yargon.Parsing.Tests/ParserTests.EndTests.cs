using System;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.End"/> method.
        /// </summary>
        public sealed class EndTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnNull_WhenAtEndOfInput()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();
                var tokens = CreateTokenStream<TokenType>();

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Null(result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenNotAtEndOfInput()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldSucceedWithNoMessages()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();
                var tokens = CreateTokenStream<TokenType>();

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(List.Empty<String>(), result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldFailWithAMessage()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new[] { "Expected end of input, got zero." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();
                var tokens = CreateTokenStream<TokenType>();

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(tokens, result.Remainder);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var parser = Parser.End<Token<TokenType>>();

                // Act
                var exception = Record.Exception(() =>
                {
                    parser(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
