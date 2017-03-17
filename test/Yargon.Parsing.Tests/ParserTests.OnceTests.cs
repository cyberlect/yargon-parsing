using System;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Once"/> method.
        /// </summary>
        public sealed class OnceTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>("abc");
                var parser = firstParser.Once();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "abc" }, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var parser = firstParser.Once();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnInputParserMessages_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Once();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnErrorMessage_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<String>().WithMessage("First parser error.");
                var parser = firstParser.Once();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new [] { "First parser error." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Once();

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
                    Parser.Once<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
