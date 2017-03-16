using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ITokenStream<TToken> remainder;

        /// <inheritdoc />
        public ITokenStream<TToken> Remainder
        {
            get
            {
                #region Contract
                if (!this.Successful)
                    throw new InvalidOperationException("No remainder available.");
                #endregion

                return this.remainder;
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Expectations { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="remainder">The remaining tokens; or <see langword="null"/>.</param>
        /// <param name="expectations">The names of the things expected.</param>
        public ParseResult(IResult<T> result, [CanBeNull] ITokenStream<TToken> remainder, IEnumerable<String> expectations)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            #endregion

            this.Result = result;
            this.remainder = remainder ?? TokenStream.Empty<TToken>();
            this.Expectations = expectations.ToList();
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
        internal static IParseResult<T, TToken> Or<T, TToken>(this IParseResult<T, TToken> first, IParseResult<T, TToken> second)
        {
            var messages = first.Messages.Concat(second.Messages).ToArray();
            
            if (first.Successful)
                return first;
            else if (second.Successful)
                return second;
            else if (first.Remainder.Count < second.Remainder.Count)
                return first;
            else if (first.Remainder.Count > second.Remainder.Count)
                return second;
            else
                return ParseResult.Fail<T, TToken>(first.Expectations.Concat(second.Expectations), first.Messages.Concat(second.Messages));
        }

        internal static IParseResult<U, TToken> And<T, U, TToken>(this IParseResult<T, TToken> first, IParseResult<U, TToken> second)
        {
            var expectations = first.Expectations.Concat(second.Expectations);
            var messages = first.Messages.Concat(second.Messages).ToArray();

            if (first.Successful && second.Successful)
                return ParseResult.Success(second.Remainder, expectations, second.Value, messages);
            else
                return ParseResult.Fail<U, TToken>(expectations, messages);
        }

        /// <summary>
        /// Creates a successful parse result with the specified result and remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="expectations">The names of the things expected.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, IEnumerable<String> expectations, [CanBeNull] T value)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            #endregion

            return Success(remainder, expectations, value, List.Empty<String>());
        }

        /// <summary>
        /// Creates a successful parse result with the specified result, remaining tokens, and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="expectations">The names of the things expected.</param>
        /// <param name="value">The resulting value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, IEnumerable<String> expectations, [CanBeNull] T value,
            params string[] messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Success(remainder, expectations, value, (IEnumerable<String>) messages);
        }

        /// <summary>
        /// Creates a successful parse result with the specified result, remaining tokens, and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="expectations">The names of the things expected.</param>
        /// <param name="value">The resulting value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(ITokenStream<TToken> remainder, IEnumerable<String> expectations, [CanBeNull] T value,
            IEnumerable<String> messages)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new ParseResult<T, TToken>(Result.Success(value, messages), remainder, expectations);
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="expectations">The names of the things expected.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(IEnumerable<String> expectations)
        {
            #region Contract
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            #endregion

            return Fail<T, TToken>(expectations, List.Empty<String>());
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="expectations">The names of the things expected.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(IEnumerable<String> expectations, params string[] messages)
        {
            #region Contract
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Fail<T, TToken>(expectations, (IEnumerable<String>)messages);
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="expectations">The names of the things expected.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(IEnumerable<String> expectations, IEnumerable<String> messages)
        {
            #region Contract
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new ParseResult<T, TToken>(Result.Fail<T>(messages), null, expectations);
        }
    }
}
