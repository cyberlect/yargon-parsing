using System;
using System.Linq;
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
                Assert.Empty(result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessages_WhenGivenMessages()
            {
                // Arrange
                var parser = Parser.Fail<String, Token<TokenType>>().WithMessage(Message.Error("Error message."));
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new [] { "Error message." }, result.Messages.Select(m => m.Text));
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
