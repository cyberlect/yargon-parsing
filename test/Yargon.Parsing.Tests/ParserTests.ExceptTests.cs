using System;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Except"/> method.
        /// </summary>
        public sealed class ExceptTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceed_WhenExceptParserFailsAndFirstParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var exceptParser = FailParser<String>();
                var parser = firstParser.Except(exceptParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenExceptParserFailsAndFirstParserFails()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var exceptParser = FailParser<String>();
                var parser = firstParser.Except(exceptParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenExceptParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var exceptParser = SuccessParser<String>();
                var parser = firstParser.Except(exceptParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnMessage_WhenExceptParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var exceptParser = SuccessParser<String>();
                var parser = firstParser.Except(exceptParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new [] { "Parser should not have succeeded." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfFirstParser_WhenExceptParserAndFirstParserFail()
            {
                // Arrange
                var firstParser = FailParser<String>("First parser error.");
                var exceptParser = FailParser<String>("Except parser error.");
                var parser = firstParser.Except(exceptParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new[] { "First parser error." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var exceptParser = SuccessParser<String>();
                var parser = firstParser.Except(exceptParser);

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
                // Arrange
                var exceptParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Except<String, String, Token<TokenType>>(null, exceptParser);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenExceptParserIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    firstParser.Except<String, String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
