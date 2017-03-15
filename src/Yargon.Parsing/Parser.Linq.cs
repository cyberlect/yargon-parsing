﻿using System;

namespace Yargon.Parsing
{
    partial class Parser
    {
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
        /// Creates a parser that succeeds only when the specified parser succeeds and the specified condition succeeds.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The parser condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> Where<TResult, TToken>(this Parser<TResult, TToken> parser, Predicate<TResult> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            IParseResult<TResult, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var result = parser(input);

                if (result.Successful)
                {
                    if (condition(result.Value))
                        return result;
                    else
                        return ParseResult.Fail<TResult, TToken>(input, "Unexpected.");
                }
                else
                    return result;
            }

            return Parser;
        }
    }
}
