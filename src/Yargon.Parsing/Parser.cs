using System;
using System.Linq;
using JetBrains.Annotations;

namespace Yargon.Parsing
{
    // This code was heavily inspired by and based upon
    // the parser combinators in the Sprache library
    // originally written by Nicholas Blumhardt,
    // licensed under the MIT license.

    /// <summary>
    /// A parser, that takes a token stream and produces a result and the remaining input.
    /// </summary>
    /// <typeparam name="T">The type of result of the parser.</typeparam>
    /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
    /// <param name="input">The token stream.</param>
    /// <returns>The parse result.</returns>
    public delegate IParseResult<T, TToken> Parser<out T, TToken>(ITokenStream<TToken> input);

    /// <summary>
    /// Functions for working with parsers.
    /// </summary>
    /// <remarks>
    /// When a parser function fails, it will fail with a message explaining
    /// what unexpected things the parser encountered. For best results, ensure
    /// your tokens implement a human-readable <see cref="Object.ToString"/> method.
    /// </remarks>
    public static partial class Parser
    {
        /// <summary>
        /// Creates a parse error message.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="text">The message text.</param>
        /// <param name="tokens">The token stream where the error occurred.</param>
        /// <returns>The resulting message.</returns>
        internal static Message Error<TToken>(string text, ITokenStream<TToken> tokens)
        {
            // TODO: Source location.
            return Message.Error(text);
        }

        /// <summary>
        /// Creates a parser that negates the result of the parser.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<object, TToken> Not<T, TToken>(
            this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<object, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);

                if (result.Successful)
                {
                    string message = result.Expectations.Any()
                        ? $"Unexpected {String.Join(", ", result.Expectations)}."
                        : "Unexpected token.";

                    return ParseResult.Fail<object, TToken>(input)
                        .WithMessage(Error(message, result.Remainder));
                }
                else
                {
                    return ParseResult.Success(default(object), input);
                }
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that executes the parser returned from the specified function after this parser.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the second parser.</typeparam>
        /// <typeparam name="T">The type of result of the first parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The first parser.</param>
        /// <param name="f">A function returning the second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Then<TResult, T, TToken>(
            this Parser<T, TToken> parser,
            Func<T, Parser<TResult, TToken>> f)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (f == null)
                throw new ArgumentNullException(nameof(f));
            #endregion

            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);
                if (!result.Successful)
                {
                    return ParseResult.Fail<TResult, TToken>(result.Remainder)
                        .WithMessages(result.Messages)
                        .WithExpectations(result.Expectations);
                }
                else
                {
                    return f(result.Value)(result.Remainder)
                        .WithMessages(result.Messages);
                }
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that discards the result on success
        /// and executes the specified parser after this parser.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the second parser.</typeparam>
        /// <typeparam name="T">The type of result of the first parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Then<TResult, T, TToken>(
            this Parser<T, TToken> first,
            Parser<TResult, TToken> second)
        {
            #region Contract
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            #endregion

            return first.Then(_ => second);
        }

        /// <summary>
        /// Creates a parser that tries the first parser, and if it fails, continues with the second parser.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parsers.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Otherwise<TResult, TToken>(
            this Parser<TResult, TToken> first,
            Parser<TResult, TToken> second)
        {
            #region Contract
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            #endregion

            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result1 = first(input);
                if (result1.Successful)
                    return result1;

                var result2 = second(input);

                return result1.Or(result2);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that applies the parser except when the specified parser succeeds.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TExcept">The type of result of the parser condition.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="except">The parser that must not succeed for the parser to be applied.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Except<TResult, TExcept, TToken>(
            this Parser<TResult, TToken> parser,
            Parser<TExcept, TToken> except)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (except == null)
                throw new ArgumentNullException(nameof(except));
            #endregion

            return except.Not().Then(parser);
        }

        /// <summary>
        /// Creates a parser with the specified name, as used in error messages.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="description">A description of the value; or <see langword="null"/> to specify none.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Named<TResult, TToken>(this Parser<TResult, TToken> parser,
            [CanBeNull] string description)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return parser(input)
                    .WithExpectation(description);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that returns the specified message.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="message">The message; or <see langword="null"/> to specify none.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> WithMessage<TResult, TToken>(this Parser<TResult, TToken> parser, [CanBeNull] IMessage message)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return parser(input)
                    .WithMessage(message);
            }

            return Parser;
        }
    }
}
