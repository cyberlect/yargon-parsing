using System;
using System.Collections.Generic;
using System.Linq;

namespace Yargon.Parsing
{
    partial class Parser
    {
        /// <summary>
        /// Creates a parser that parses a sequence of zero or one element.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<T>, TToken> Maybe<T, TToken>(this Parser<T, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            return parser.Once().Otherwise(Return<IEnumerable<T>, TToken>(Enumerable.Empty<T>()));
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

            return parser.Once().Concat(parser.Many());
        }

        /// <summary>
        /// Creates a parser that parses a sequence of zero or more elements.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<TResult>, TToken> Many<TResult, TToken>(this Parser<TResult, TToken> parser)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            #endregion

            IParseResult<IEnumerable<TResult>, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var results = new List<IParseResult<TResult, TToken>>();
                var remainder = input;
                var result = parser(remainder);
                while (result.Successful)
                {
                    results.Add(result);
                    remainder = result.Remainder;
                    result = parser(remainder);
                }
                
                return ParseResult.Success(results.Select(r => r.Value), remainder)
                    .WithExpectation($"many of {String.Join(", ", results.SelectMany(r => r.Expectations).Distinct())}")
                    .WithMessages(results.SelectMany(r => r.Messages));
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that parses a sequence until the specified parser succeeds.
        /// </summary>
        /// <typeparam name="TResult">The type of result of the parser.</typeparam>
        /// <typeparam name="TUntil">The type of result of the parser condition.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="parser">The parser to repeat.</param>
        /// <param name="until">The parser that must succeed for the sequence to end.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<TResult>, TToken> Until<TResult, TUntil, TToken>(this Parser<TResult, TToken> parser, Parser<TUntil, TToken> until)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (until == null)
                throw new ArgumentNullException(nameof(until));
            #endregion
            
            return parser.Except(until).Many();
        }

        /// <summary>
        /// Creates a parser that concatenates the results of two sequence parsers.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The created parser.</returns>
        public static Parser<IEnumerable<T>, TToken> Concat<T, TToken>(this Parser<IEnumerable<T>, TToken> first, Parser<IEnumerable<T>, TToken> second)
        {
            #region Contract
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            #endregion

            return first.Then(s => second.Select(s.Concat));
        }

        /// <summary>
        /// Creates a parser that parses a sequence with an exact number of repetitions.
        /// </summary>
        /// <param name="parser">The parser to repeat.</param>
        /// <param name="count">The number of repetitions.</param>
        /// <returns>The parser.</returns>
        public static Parser<IEnumerable<TResult>, TToken> Take<TResult, TToken>(this Parser<TResult, TToken> parser, int count)
        {
            #region Contract
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            #endregion

            IParseResult<IEnumerable<TResult>, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                var results = new List<IParseResult<TResult, TToken>>();
                var remainder = input;
                for (int i = 0; i < count; i++)
                {
                    var result = parser(remainder);
                    if (!result.Successful)
                    {
                        string message = result.Remainder.AtEnd
                            ? "Unexpected end of input."
                            : $"Unexpected {result.Remainder.Current}.";

                        return ParseResult.Fail<IEnumerable<TResult>, TToken>(input)
                            .WithMessage(Error(message, result.Remainder))
                            .WithExpectation($"{count} repetitions of {String.Join(", ", result.Expectations)}");
                    }

                    results.Add(result);
                    remainder = result.Remainder;
                }

                return ParseResult.Success(results.Select(r => r.Value), remainder)
                    .WithExpectation($"{count} repetitions of {String.Join(", ", results.SelectMany(r => r.Expectations).Distinct())}")
                    .WithMessages(results.SelectMany(r => r.Messages));
            }
            
            return Parser;
        }
    }
}
