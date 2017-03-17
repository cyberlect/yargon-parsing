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
        /// Tests the <see cref="Parser.Take"/> method.
        /// </summary>
        public sealed class TakeTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhenAllParsingsSucceed()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => true);
                var parser = firstParser.Take(2);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { TokenType.Zero, TokenType.One }, result.Value.Select(t => t.Type));
            }

            [Fact]
            public void ReturnedParser_ShouldAlwaysSucceed_WhenCountIsZero()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => true);
                var parser = firstParser.Take(0);
                var tokens = CreateTokenStream<TokenType>();

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenNotAllParsingsSucceed()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => true);
                var parser = firstParser.Take(4);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnErrorMessages_WhenItFails()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => true);
                var parser = firstParser.Take(4);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new [] { "Unexpected end of input." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var secondParser = SuccessParser<String>();
                var parser = firstParser.Take(1);

                // Act
                var exception = Record.Exception(() =>
                {
                    parser(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentOutOfRangeException_WhenInputIsNegative()
            {
                // Arrange
                var firstParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    firstParser.Take(-3);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentOutOfRangeException>(exception);
            }
        }
    }
}
