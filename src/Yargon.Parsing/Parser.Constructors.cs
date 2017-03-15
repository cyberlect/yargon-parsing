using System;
using System.Collections.Generic;
using System.Text;
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
                    return ParseResult.Success(input, default(object));
                else
                    return ParseResult.Fail<object, TToken>(input, "Unexpected token, expected end of input.");
            }

            return Parser;
        }
    }
}
