using System;
using System.Collections.Generic;
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
    public static partial class Parser
    {
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
                    return ParseResult.Fail<object, TToken>(input, "Not expected.");
                else
                    return ParseResult.Success(input, (object) null);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that executes the specified parser after this parser.
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
        /// Creates a parser that executes the specified parser after this parser.
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

                if (result.Successful)
                    return f(result.Value)(result.Remainder);
                else
                    return ParseResult.Fail<TResult, TToken>(input);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that tries the first parser, and if it fails, continues with the second parser.
        /// </summary>
        /// <typeparam name="TReturn">The type of result of the parsers.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TReturn, TToken> Or<TReturn, TToken>(
            this Parser<TReturn, TToken> first,
            Parser<TReturn, TToken> second)
        {
            #region Contract
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            #endregion

            IParseResult<TReturn, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result1 = first(input);

                if (result1.Successful)
                {
                    return result1;
                }
                else
                {
                    // TODO: Merge messages on failure.
                    return second(input);
                }
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that discards the input parser's result and returns its own.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <typeparam name="TDiscard">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The first parser.</param>
        /// <param name="value">The actual value to return.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Return<TResult, TDiscard, TToken>(
            this Parser<TDiscard, TToken> parser,
            [CanBeNull] TResult value)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            return parser.Select(t => value);
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
            
            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = except(input);

                if (!result.Successful)
                    return parser(input);
                
                return ParseResult.Fail<TResult, TToken>(input, "Parser should not have succeeded.");
            }

            return Parser;
        }
    }
}
