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
        /// Tests the <see cref="Parser.Until"/> method.
        /// </summary>
        public sealed class UntilTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhileUntilParserFails()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.Zero);
                var untilParser = Parser.Token<Token<TokenType>>(t => t.Type == TokenType.One);
                var parser = firstParser.Until(untilParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { TokenType.Zero }, result.Value.Select(t => t.Type));
            }

            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnEmptySequence_WhenUntilParserSucceedsImmediately()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var untilParser = SuccessParser<String>();
                var parser = firstParser.Until(untilParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Value);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnConcatenatedInputParserMessages_WhileUntilParserSucceeds()
            {
                // Arrange
                var firstParser = Parser.Token<Token<TokenType>>(t => true).WithMessage(Message.Error("Parser message."));
                var untilParser = FailParser<String>();
                var parser = firstParser.Until(untilParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "Parser message.", "Parser message.", "Parser message." }, result.Messages.Select(m => m.Text));
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var untilParser = SuccessParser<String>();
                var parser = firstParser.Until(untilParser);

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
                    Parser.Until<String, String, Token<TokenType>>(null, SuccessParser<String>());
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenUntilParserIsNull()
            {
                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Until<String, String, Token<TokenType>>(SuccessParser<String>(), null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
