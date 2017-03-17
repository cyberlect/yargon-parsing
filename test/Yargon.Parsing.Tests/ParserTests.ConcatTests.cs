using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Concat"/> method.
        /// </summary>
        public sealed class ConcatTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndConcatenateSequences_WhenBothInputParsersSucceed()
            {
                // Arrange
                var firstParser = SuccessParser(new[] {"a", "b"});
                var secondParser = SuccessParser(new[] { "c", "d" });
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "a", "b", "c", "d" }, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenFirstInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<IEnumerable<String>>();
                var secondParser = SuccessParser(new[] { "c", "d" });
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenSecondInputParserFails()
            {
                // Arrange
                var firstParser = SuccessParser(new[] { "a", "b" });
                var secondParser = FailParser<IEnumerable<String>>();
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldFail_WhenBothInputParsersFail()
            {
                // Arrange
                var firstParser = FailParser<IEnumerable<String>>();
                var secondParser = FailParser<IEnumerable<String>>();
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }

            [Fact]
            public void ReturnedParser_ShouldConcatenatedInputParserMessages_WhenInputParsersSucceed()
            {
                // Arrange
                var firstParser = SuccessParser(new[] { "a", "b" }).WithMessage("First parser message.");
                var secondParser = SuccessParser(new[] { "c", "d" }).WithMessage("Second parser message.");
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "First parser message.", "Second parser message." }.OrderBy(m => m), result.Messages.OrderBy(m => m));
            }

            [Fact]
            public void ReturnedParser_ShouldReturnFirstInputMessages_WhenFirstInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<IEnumerable<String>>().WithMessage("First parser message.");
                var secondParser = SuccessParser(new[] { "c", "d" }).WithMessage("Second parser message.");
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new[] { "First parser message." }, result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnFirstAndSecondInputMessages_WhenSecondInputParserFails()
            {
                // Arrange
                var firstParser = SuccessParser(new[] { "a", "b" }).WithMessage("First parser message.");
                var secondParser = FailParser<IEnumerable<String>>().WithMessage("Second parser message.");
                var parser = firstParser.Concat(secondParser);
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
                Assert.Equal(new[] { "First parser message.", "Second parser message." }.OrderBy(m => m), result.Messages.OrderBy(m => m));
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var secondParser = SuccessParser<String>();
                var parser = firstParser.Concat(secondParser);

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
                    Parser.Concat(null, SuccessParser<IEnumerable<String>>());
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
                    Parser.Concat(SuccessParser<IEnumerable<String>>(), null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
