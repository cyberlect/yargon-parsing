using System;
using System.Linq;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Token"/> method.
        /// </summary>
        public sealed class TokenTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnToken_WhenPredicateIsTrueForNextToken()
            {
                // Arrange
                var parser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(tokens.First(), result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenPredicateIsFalseForNextToken()
            {
                // Arrange
                var parser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.One);
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
                var parser = Parser.Token<Token<TokenType>>(t => true);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(List.Empty<String>(), result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldFailWithAMessage()
            {
                // Arrange
                var parser = Parser.Token<Token<TokenType>>(t => false);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new [] { "Expected zero, got zero." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldConsumeOneToken_WhenItSucceeds()
            {
                // Arrange
                var parser = Parser.Token<Token<TokenType>>(t => true);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(tokens.Skip(1), result.Remainder);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var parser = Parser.Token<Token<TokenType>>(t => true);

                // Act
                var exception = Record.Exception(() =>
                {
                    parser(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenPredicateIsNull()
            {
                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Token<Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
