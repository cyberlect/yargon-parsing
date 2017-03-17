using System;
using JetBrains.Annotations;

namespace Yargon.Parsing
{
    partial class Parser
    {
        /// <summary>
        /// Creates a parser that always succeeds and returns a value
        /// without consuming any input.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="value">The value to return.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Return<T, TToken>([CanBeNull] T value)
            => Return<T, TToken>(value, null);

        /// <summary>
        /// Creates a parser that always succeeds and returns a value
        /// without consuming any input.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="value">The value to return.</param>
        /// <param name="description">A description of the value; or <see langword="null"/> to specify none.</param>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Return<T, TToken>([CanBeNull] T value, [CanBeNull] string description)
        {
            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return ParseResult.Success(value, input)
                    .WithExpectation(description);
            }

            return Parser;
        }

        /// <summary>
        /// Creates a parser that always fails without consuming any input.
        /// </summary>
        /// <typeparam name="T">The type of result of the parser.</typeparam>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <returns>The created parser.</returns>
        public static Parser<T, TToken> Fail<T, TToken>()
        {
            IParseResult<T, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                return ParseResult.Fail<T, TToken>(input);
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
            => Token<TToken>(predicate, null);

        /// <summary>
        /// Creates a parser that succeeds when it can consume one token from
        /// the token stream.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <param name="predicate">The condition on which the tokens are parsed.</param>
        /// <param name="description">A description of the token; or <see langword="null"/> to specify none.</param>
        /// <returns>The created parser.</returns>
        public static Parser<TToken, TToken> Token<TToken>(Predicate<TToken> predicate, [CanBeNull] string description)
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
                {
                    return ParseResult.Fail<TToken, TToken>(input)
                        .WithMessage(Error("Unexpected end of input.", input));
                }

                var token = input.Current;
                if (predicate(token))
                {
                    return ParseResult.Success(token, input.Advance());
                }
                else
                {
                    return ParseResult.Fail<TToken, TToken>(input)
                        .WithMessage(Error($"Unexpected token {input.Current}.", input));
                }
            }

            Parser<TToken, TToken> parser = Parser;
            return parser.Named(description);
        }

        /// <summary>
        /// Creates a parser that succeeds when the end of the input has been reached.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the token stream.</typeparam>
        /// <returns>The created parser.</returns>
        public static Parser<object, TToken> End<TToken>()
        {
            IParseResult<object, TToken> Parser(ITokenStream<TToken> input)
            {
                #region Contract
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                #endregion

                if (input.AtEnd)
                {
                    return ParseResult.Success(default(object), input)
                        .WithExpectation("end of input");
                }
                else
                {
                    return ParseResult.Fail<object, TToken>(input)
                        .WithMessage(Error($"Unexpected token {input.Current}.", input))
                        .WithExpectation("end of input");
                }
            }
            
            Parser<object, TToken> parser = Parser;
            return parser.Named("end of input");
        }
    }
}
