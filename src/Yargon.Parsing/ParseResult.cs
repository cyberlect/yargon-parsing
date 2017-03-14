using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Virtlink.Utilib.Collections;

namespace Yargon.Parsing
{
    /// <summary>
    /// A parse result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TToken">The type of tokens.</typeparam>
    public sealed class ParseResult<T, TToken> : IParseResult<T, TToken>
    {
        /// <inheritdoc />
        public IResult<T> Result { get; }

        /// <inheritdoc />
        public bool Successful => this.Result.Successful;

        /// <inheritdoc />
        public T Value => this.Result.Value;

        /// <inheritdoc />
        public IReadOnlyCollection<string> Messages => this.Result.Messages;

        /// <inheritdoc />
        public ITokenStream<TToken> Remainder { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="remainder">The remaining tokens.</param>
        public ParseResult(IResult<T> result, ITokenStream<TToken> remainder)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            #endregion

            this.Result = result;
            this.Remainder = remainder;
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public bool Equals(ParseResult<T, TToken> other)
        {
            return !Object.ReferenceEquals(other, null)
                && Object.Equals(this.Result, other.Result)
                && Object.Equals(this.Remainder, other.Remainder);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.Result.GetHashCode();
                hash = hash * 29 + this.Remainder.GetHashCode();
            }
            return hash;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ParseResult<T, TToken>);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="ParseResult{T, TToken}"/> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ParseResult<T, TToken> left, ParseResult<T, TToken> right) => Object.Equals(left, right);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="ParseResult{T, TToken}"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ParseResult<T, TToken> left, ParseResult<T, TToken> right) => !(left == right);
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Result} {{{this.Remainder}}}";
        }
    }

    /// <summary>
    /// Functions for working with <see cref="IParseResult{T, TToken}"/> objects.
    /// </summary>
    public static class ParseResult
    {
        /// <summary>
        /// Creates a successful parse result with the specified result and remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, [CanBeNull] T value)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            #endregion

            return Success(remainder, value, List.Empty<String>());
        }

        /// <summary>
        /// Creates a successful parse result with the specified result, remaining tokens, and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="value">The resulting value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, [CanBeNull] T value,
            params string[] messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Success(remainder, value, (IReadOnlyCollection<String>) messages);
        }

        /// <summary>
        /// Creates a successful parse result with the specified result, remaining tokens, and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="value">The resulting value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, [CanBeNull] T value,
            IReadOnlyCollection<String> messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new ParseResult<T, TToken>(Result.Success(value, messages), remainder);
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(ITokenStream<TToken> remainder)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            #endregion

            return Fail<T, TToken>(remainder, List.Empty<String>());
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(ITokenStream<TToken> remainder, params string[] messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Fail<T, TToken>(remainder, (IReadOnlyCollection<String>)messages);
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(ITokenStream<TToken> remainder, IReadOnlyCollection<String> messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new ParseResult<T, TToken>(Result.Fail<T>(messages), remainder);
        }
    }
}
