using System;
using System.Linq;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Not"/> method.
        /// </summary>
        public sealed class NotTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnNull_WhenGivenParserFails()
            {
                // Arrange
                var originalParser = FailParser<String>();
                var parser = originalParser.Not();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Null(result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenGivenParserSucceeds()
            {
                // Arrange
                var originalParser = SuccessParser<String>();
                var parser = originalParser.Not();
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
                var originalParser = FailParser<String>();
                var parser = originalParser.Not();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Empty(result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldFailWithAMessage()
            {
                // Arrange
                var originalParser = SuccessParser<String>().Named("success");
                var parser = originalParser.Not();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new[] { "Unexpected success." }, result.Messages.Select(m => m.Text));
            }

            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput_WhenItSucceeds()
            {
                // Arrange
                var originalParser = ConsumingParser(1).Then(FailParser<String>());
                var parser = originalParser.Not();
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
                var originalParser = FailParser<String>();
                var parser = originalParser.Not();

                // Act
                var exception = Record.Exception(() =>
                {
                    parser(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenInputParserIsNull()
            {
                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Not<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
