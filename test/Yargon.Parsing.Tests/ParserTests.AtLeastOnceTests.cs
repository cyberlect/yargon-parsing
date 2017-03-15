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
        /// Tests the <see cref="Parser.AtLeastOnce"/> method.
        /// </summary>
        public sealed class AtLeastOnceTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.AtLeastOnce();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { TokenType.Zero }, result.Value.Select(t => t.Type));
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenInputParserFailsImmediately()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var parser = firstParser.AtLeastOnce();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnConcatenatedInputParserMessages_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var parser = firstParser.AtLeastOnce();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "" }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnErrorMessage_WhenInputParserFailsImmediately()
            {
                // Arrange
                var firstParser = FailParser<String>("First parser error.");
                var parser = firstParser.AtLeastOnce();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new [] { "First parser error." }, result.Messages);
            }
            
            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput_WhenItFails()
            {
                // Arrange
                var firstParser = ConsumingParser(1).Then(FailParser<String>());
                var parser = firstParser.AtLeastOnce();
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
                var firstParser = SuccessParser<String>();
                var parser = firstParser.AtLeastOnce();

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
                    Parser.AtLeastOnce<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
