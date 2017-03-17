using System;

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
    }
}
