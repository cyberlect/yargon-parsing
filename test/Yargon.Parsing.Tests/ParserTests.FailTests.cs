using System;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Fail"/> method.
        /// </summary>
        public sealed class FailTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldFail()
            {
                // Arrange
                var parser = Parser.Fail<String, Token<TokenType>>();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldByDefaultReturnNoMessages()
            {
                // Arrange
                var parser = Parser.Fail<String, Token<TokenType>>();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(List.Empty<String>(), result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessages_WhenGivenMessages()
            {
                // Arrange
                string message = "Error message.";
                var parser = Parser.Fail<String, Token<TokenType>>(message);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new [] { message }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput()
            {
                // Arrange
                var parser = Parser.Fail<String, Token<TokenType>>();
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
                var parser = Parser.Fail<String, Token<TokenType>>();

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
