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
        /// Tests the <see cref="Parser.Select"/> method.
        /// </summary>
        public sealed class SelectTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldProjectTheResult_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.Select(t => t.Type);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(TokenType.Zero, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<Token<TokenType>>();
                var parser = firstParser.Select(t => t.Type);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Select(t => t);

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
                    Parser.Select<String, String, Token<TokenType>>(null, t => t);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenProjectionFunctionIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    firstParser.Select<String, String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
