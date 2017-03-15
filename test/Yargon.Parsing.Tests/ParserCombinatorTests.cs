using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// Base class for parser combinator tests.
    /// </summary>
    public abstract class ParserCombinatorTests
    {
        /// <summary>
        /// Creates a parser that always succeeds.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The parser.</returns>
        protected static Parser<T, Token<TokenType>> SuccessParser<T>()
            => SuccessParser(default(T));

        /// <summary>
        /// Creates a parser that always succeeds.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The parser.</returns>
        protected static Parser<T, Token<TokenType>> SuccessParser<T>(T value)
            => Parser.Return<T, Token<TokenType>>(value);
        
        /// <summary>
        /// Creates a parser that always fails.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="messages">The error messages.</param>
        /// <returns>The parser.</returns>
        protected static Parser<T, Token<TokenType>> FailParser<T>(params string[] messages)
            => Parser.Fail<T, Token<TokenType>>(messages);

        /// <summary>
        /// Creates a parser that consumes a fixed number of tokens.
        /// </summary>
        /// <param name="count">The number of tokens.</param>
        /// <returns>The parser.</returns>
        protected static Parser<IEnumerable<Token<TokenType>>, Token<TokenType>> ConsumingParser(int count)
            => Parser.Take(Parser.Token<Token<TokenType>>(t => true), count);

        /// <summary>
        /// Creates a token stream. The tokens themselves have no value and no source range.
        /// </summary>
        /// <typeparam name="TTokenType">The type of token types.</typeparam>
        /// <param name="tokenTypes">The token types.</param>
        /// <returns>The token stream.</returns>
        protected static ITokenStream<Token<TTokenType>> CreateTokenStream<TTokenType>(params TTokenType[] tokenTypes)
        {
            #region Contract
            Debug.Assert(tokenTypes != null);
            #endregion

            return new TokenSequence<Token<TTokenType>>(
                    tokenTypes.Select(tt => new Token<TTokenType>(String.Empty, tt, SourceRange.Empty)));
        }
    }
}
