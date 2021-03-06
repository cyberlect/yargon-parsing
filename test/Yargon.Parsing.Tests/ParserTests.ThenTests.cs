﻿using System;
using System.Linq;
using Virtlink.Utilib.Collections;
using Xunit;

namespace Yargon.Parsing
{
    partial class ParserTests
    {
        /// <summary>
        /// Tests the <see cref="Parser.Then"/> methods.
        /// </summary>
        public sealed class ThenTests : ParserCombinatorTests
        {
            [Fact]
            public void ReturnedParser_ShouldReturnResultOfSecondParser_WhenFirstParserSucceeds()
            {
                // Arrange
                string input = "abc";
                var parser = SuccessParser<Int32>().Then(v => Parser.Return<String, Token<TokenType>>(input));
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.True(result.Successful);
                Assert.Equal(input, result.Value);
            }

            [Fact]
            public void ReturnedParser_ShouldReturnResultOfFirstParser_WhenFirstParserFails()
            {
                // Arrange
                var parser = FailParser<Int32>().Then(v => Parser.Return<String, Token<TokenType>>("abc"));
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.False(result.Successful);
            }
            
            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfFirstParser_WhenFirstParserFails()
            {
                // Arrange
                var parser = Parser.Fail<Int32, Token<TokenType>>().WithMessage(Message.Error("Some message.")).Then(v => Parser.Return<String, Token<TokenType>>("abc"));
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new[] { "Some message." }, result.Messages.Select(m => m.Text));
            }

            [Fact]
            public void ReturnedParser_ShouldReturnMessagesOfFirstAndSecondParser_WhenFirstParserSucceeds()
            {
                // Arrange
                var parser = SuccessParser<Int32>().WithMessage(Message.Error("First message."))
                    .Then(v => Parser.Fail<String, Token<TokenType>>().WithMessage(Message.Error("Second message.")));
                var tokens = CreateTokenStream(TokenType.Zero, TokenType.One, TokenType.Zero);

                // Act
                var result = parser(tokens);

                // Assert
                Assert.Equal(new[] { "First message.", "Second message." }.OrderBy(m => m), result.Messages.OrderBy(m => m.Text).Select(m => m.Text));
            }

            [Fact]
            public void ReturnedParser_ShouldThrowArgumentNullException_WhenInputIsNull()
            {
                // Arrange
                var parser = SuccessParser<Int32>().Then(v => SuccessParser<String>());

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
                    Parser.Then<String, Int32, Token<TokenType>>(null, v => SuccessParser<String>());
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenSecondParserIsNull()
            {
                // Act
                var exception = Record.Exception(() =>
                {
                    SuccessParser<Int32>().Then((Func<Int32, Parser<String, Token<TokenType>>>)null);
                });

                // Assert
                Assert.IsAssignableFrom<ArgumentNullException>(exception);
            }
        }
    }
}
