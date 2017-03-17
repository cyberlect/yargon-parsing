using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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
        /// <typeparam name="TCollection">The type of intermediate result.</typeparam>
        /// <typeparam name="TResult">The type of element in the resulting sequence.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="collectionSelector">The function to apply to each result.</param>
        /// <param name="resultSelector">The function to apply to each element of each result.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TResult, TToken> SelectMany<TSource, TCollection, TResult, TToken>(
            this Parser<TSource, TToken> parser,
            Func<TSource, Parser<TCollection, TToken>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (collectionSelector == null)
                throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            #endregion

            return parser.Then(t => collectionSelector(t).Select(u => resultSelector(t, u)));
        }

        /// <summary>
        /// Creates a parser that succeeds only when the specified parser succeeds and the specified condition succeeds.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
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
                return result.OnSuccess(r =>
                {
                    if (!condition(result.Value))
                    {
                        return ParseResult.Fail<TResult, TToken>(input)
                            .WithMessage(Error($"Unexpected {String.Join(", ", r.Expectations)}", result.Remainder));
                    }

                    return r;
                });
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that returns the first result for which the condition succeeds,
        /// or the specified default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> FirstOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition,
            [CanBeNull] T defaultValue)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Where(v => condition(v)).DefaultIfEmpty(defaultValue).FirstOrDefault());
        }

        /// <summary>
        /// Creates a parser that returns the first result for which the condition succeeds,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> FirstOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
            => parser.FirstOrDefault(condition, default(T));

        /// <summary>
        /// Creates a parser that returns the first result,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> FirstOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.FirstOrDefault(v => true, default(T));

        /// <summary>
        /// Creates a parser that returns the first result for which the condition succeeds.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> First<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.First(v => condition(v)));
        }

        /// <summary>
        /// Creates a parser that returns the first result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> First<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.First(v => true);

        /// <summary>
        /// Creates a parser that returns the last result for which the condition succeeds,
        /// or the specified default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> LastOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition,
            [CanBeNull] T defaultValue)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Where(v => condition(v)).DefaultIfEmpty(defaultValue).LastOrDefault());
        }

        /// <summary>
        /// Creates a parser that returns the last result for which the condition succeeds,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> LastOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
            => parser.LastOrDefault(condition, default(T));

        /// <summary>
        /// Creates a parser that returns the last result,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> LastOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.LastOrDefault(v => true, default(T));

        /// <summary>
        /// Creates a parser that returns the last result for which the condition succeeds.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Last<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Last(v => condition(v)));
        }

        /// <summary>
        /// Creates a parser that returns the last result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Last<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.Last(v => true);

        /// <summary>
        /// Creates a parser that returns the only result for which the condition succeeds,
        /// or the specified default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> SingleOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition,
            [CanBeNull] T defaultValue)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Where(v => condition(v)).DefaultIfEmpty(defaultValue).SingleOrDefault());
        }

        /// <summary>
        /// Creates a parser that returns the only result for which the condition succeeds,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> SingleOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
            => parser.SingleOrDefault(condition, default(T));

        /// <summary>
        /// Creates a parser that returns the only result,
        /// or the default value when there are none.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> SingleOrDefault<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.SingleOrDefault(v => true, default(T));

        /// <summary>
        /// Creates a parser that returns the only result for which the condition succeeds.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Single<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Single(v => condition(v)));
        }

        /// <summary>
        /// Creates a parser that returns the only result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Single<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.Single(v => true);

        /// <summary>
        /// Creates a parser that returns whether there is any result for which the condition succeeds.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<bool, TToken> Any<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.Any(v => condition(v)));
        }

        /// <summary>
        /// Creates a parser that returns whether there is any result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<bool, TToken> Any<T, TToken>(this Parser<IEnumerable<T>, TToken> parser)
            => parser.Any(v => true);

        /// <summary>
        /// Creates a parser that returns whether there the condition succeeds for all results.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>The created parser.</returns>
        public static Parser<bool, TToken> All<T, TToken>(this Parser<IEnumerable<T>, TToken> parser, Predicate<T> condition)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            #endregion

            return parser.Select(e => e.All(v => condition(v)));
        }
    }
}
