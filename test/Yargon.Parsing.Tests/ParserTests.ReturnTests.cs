using System;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Return{T, TToken}(T)"/> method.
        /// </summary>
        public sealed class ReturnTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnTheGivenValue()
            {
                // Arrange
                string value = "abc";
                var parser = Parser.Return<String, Token<TokenType>>(value);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(value, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnNull_WhenGivenNull()
            {
                // Arrange
                var parser = Parser.Return<String, Token<TokenType>>(null);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Null(result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldByDefaultReturnNoMessages()
            {
                // Arrange
                var parser = Parser.Return<String, Token<TokenType>>("abc");
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Empty(result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput()
            {
                // Arrange
                var parser = Parser.Return<String, Token<TokenType>>("abc");
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(tokens, result.Remainder);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var parser = Parser.Return<String, Token<TokenType>>("abc");

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
