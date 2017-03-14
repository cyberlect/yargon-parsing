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
    public static class Parser
    {
        // Parser Constructors

        /// <summary>
        /// Creates a parser that always succeeds and returns a value
        /// without consuming any input.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="value">The value to return.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Return<T, TToken>([CanBeNull] T value)
        {
            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return ParseResult.Success(input, value);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that always fails without consuming any input.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="messages">The messages.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Fail<T, TToken>(params string[] messages)
        {
            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return ParseResult.Fail<T, TToken>(input, messages);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that succeeds when it can consume one token from
        /// the token stream.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="predicate">The condition on which the tokens are parsed.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TToken, TToken> Token<TToken>(Predicate<TToken> predicate)
        {
            #region Contract
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            #endregion

            IParseResult<TToken, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                if (input.AtEnd)
                    return ParseResult.Fail<TToken, TToken>(input, "Unexpected end of input.");

                var token = input.Current;
                if (predicate(token))
                    return ParseResult.Success(input.Advance(), token);
                else
                    return ParseResult.Fail<TToken, TToken>(input, "Unexpected token.");
            }

            return Parser;
        }



        /// <summary>
        /// Creates a parser that projects the result of the parser.
        /// </summary>
        /// <typeparam name="TSource">The type of result of the parser.</typeparam>
        /// <typeparam name="TResult">The type of result of the function.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The first parser.</param>
        /// <param name="selector">A function applied to the result value.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Select<TSource, TResult, TToken>(
            this Parser<TSource, TToken> parser,
            Func<TSource, TResult> selector)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            #endregion

            return parser.Then(t => Return<TResult, TToken>(selector(t)));
        }

        /// <summary>
        /// Creates a parser that projects and flattens.
        /// </summary>
        /// <typeparam name="TSource">The type of result of the parser.</typeparam>
        /// <typeparam name="TCollection">The type of result of the filter.</typeparam>
        /// <typeparam name="TResult">The type of result of the projector.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="resultSelector">The filter function.</param>
        /// <param name="collectionSelector">The projector function.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> SelectMany<TSource, TCollection, TResult, TToken>(
            this Parser<TSource, TToken> parser,
            Func<TSource, Parser<TCollection, TToken>> resultSelector,
            Func<TSource, TCollection, TResult> collectionSelector)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            if (collectionSelector == null)
                throw new ArgumentNullException(nameof(collectionSelector));
            #endregion

            return parser.Then(t => resultSelector(t).Select(u => collectionSelector(t, u)));
        }

        /// <summary>
        /// Creates a parser that negates the result of the input parser.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<object, TToken> Not<T, TToken>(this Parser<T, TToken> parser)
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
        /// Creates a parser that executes the first parser and then a second parser.
        /// </summary>
        /// <typeparam name="T">The type of result of the first parser.</typeparam>
        /// <typeparam name="U">The type of result of the second parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The first parser.</param>
        /// <param name="f">A function returning the second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<U, TToken> Then<T, U, TToken>(this Parser<T, TToken> parser, Func<T, Parser<U, TToken>> f)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (f == null)
                throw new ArgumentNullException(nameof(f));
            #endregion

            IParseResult<U, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);

                if (result.Successful)
                    return f(result.Value)(input);
                else
                    return ParseResult.Fail<U, TToken>(input);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that succeeds when the end of the input has been reached.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> End<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);

                if (result.Successful)
                {
                    if (result.Remainder.AtEnd)
                        return result;
                    else
                        return ParseResult.Fail<T, TToken>(input, $"Unexpected token, expected end of input.");
                }
                else
                    return ParseResult.Fail<T, TToken>(input);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that fails when the end of the input has been reached.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The input parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> NotEnd<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);

                if (result.Successful)
                {
                    if (!result.Remainder.AtEnd)
                        return result;
                    else
                        return ParseResult.Fail<T, TToken>(input, $"Unexpected end of input.");
                }
                else
                    return ParseResult.Fail<T, TToken>(input);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that tries the first parser, and if it fails, continues with the second parser.
        /// </summary>
        /// <typeparam name="T">The type of result of the parsers.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Or<T, TToken>(this Parser<T, TToken> first, Parser<T, TToken> second)
        {
            #region Contract
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            #endregion

            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
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
                    var result2 = second(input);
                    // TODO: Merge messages on failure. (DetermineBestError())
                    return result2;
                }
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that discards the input parser's result and returns its own.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="U">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="parser">The first parser.</param>
        /// <param name="value">The actual value to return.</param>
        /// <returns>The created parser.</returns>
        public static Parser<U, TToken> Return<T, U, TToken>(this Parser<T, TToken> parser, [CanBeNull] U value)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            return parser.Select(t => value);
        }

        /// <summary>
        /// Creates a parser that parses a sequence of exactly one element.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<T>, TToken> Once<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            return parser.Select(result => new[] { result });
        }

        /// <summary>
        /// Creates a parser that parses a sequence of one or more elements.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<T>, TToken> AtLeastOnce<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            return parser.Once().Then(first => parser.Many().Select(first.Concat));
        }

        /// <summary>
        /// Creates a parser that parses a sequence of zero or more elements.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<T>, TToken> Many<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<IEnumerable<T>, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var results = new List<T>();
                var remainder = input;
                var result = parser(input);
                while (result.Successful || remainder.Equals(result.Remainder))
                {
                    results.Add(result.Value);
                    remainder = result.Remainder;
                    result = parser(remainder);
                }

                return ParseResult.Success<IEnumerable<T>, TToken>(remainder, results);
            }

            return Parser;
        }
    }
}
