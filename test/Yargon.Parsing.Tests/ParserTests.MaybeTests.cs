using System;
using System.Xml.XPath;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Maybe"/> method.
        /// </summary>
        public sealed class MaybeTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnSequenceOfResult_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>("abc");
                var parser = firstParser.Maybe();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(new [] { "abc" }, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldSucceedAndReturnEmptySequence_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<String>();
                var parser = firstParser.Maybe();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Value);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnInputParserMessages_WhenInputParserSucceeds()
            {
                // Arrange
                var firstParser = SuccessParser<String>();
                var parser = firstParser.Maybe();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Messages);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnNoMessages_WhenInputParserFails()
            {
                // Arrange
                var firstParser = FailParser<String>("First parser error.");
                var parser = firstParser.Maybe();
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Empty(result.Messages);
            }
            
            [Fact]
            public void ReturnedParser_ShouldNotConsumeAnyInput_WhenItFails()
            {
                // Arrange
                var firstParser = ConsumingParser(1).Then(FailParser<String>());
                var parser = firstParser.Maybe();
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
                var parser = firstParser.Maybe();

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
                    Parser.Maybe<String, Token<TokenType>>(null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
