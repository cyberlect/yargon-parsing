using System;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Otherwise"/> method.
        /// </summary>
        public sealed class OtherwiseTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldReturnResultOfFirstParser_WhenFirstParserSucceeds()
            {
                // Arrange
                string value = "abc";
                var firstParser = SuccessParser<String>(value);
                var secondParser = FailParser<String>();
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(value, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnResultOfSecondParser_WhenFirstParserFails()
            {
                // Arrange
                string value = "abc";
                var firstParser = FailParser<String>();
                var secondParser = SuccessParser<String>(value);
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(value, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenBothParsersFail()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var secondParser = FailParser<String>();
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfBothParsers_WhenBothParsersFailAndConsumedTheSameNumberOfTokens()
            {
                // Arrange
                var firstParser = ConsumingParser(2).Then(FailParser<String>().WithMessage("First parser error."));
                var secondParser = ConsumingParser(2).Then(FailParser<String>().WithMessage("Second parser error."));
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new [] { "First parser error.", "Second parser error." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfFirstParser_WhenBothParsersFailAndTheFirstParserConsumedMoreTokens()
            {
                // Arrange
                var firstParser = ConsumingParser(2).Then(FailParser<String>().WithMessage("First parser error."));
                var secondParser = ConsumingParser(1).Then(FailParser<String>().WithMessage("Second parser error."));
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new[] { "First parser error." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfSecondParser_WhenBothParsersFailAndTheSecondParserConsumedMoreTokens()
            {
                // Arrange
                var firstParser = ConsumingParser(1).Then(FailParser<String>().WithMessage("First parser error."));
                var secondParser = ConsumingParser(2).Then(FailParser<String>().WithMessage("Second parser error."));
                var parser = firstParser.Otherwise(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new[] { "Second parser error." }, result.Messages);
            }
            
            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var secondParser = SuccessParser<String>();
                var parser = firstParser.Otherwise(secondParser);

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
                var secondParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Otherwise(null, secondParser);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenSecondParserIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();

                // Act
                var exception = Record.Exception(() =>
                {
                    Parser.Otherwise(firstParser, null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
