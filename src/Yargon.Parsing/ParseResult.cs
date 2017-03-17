using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Successful { get; }

        /// <inheritdoc />
        public T Value { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IMessage> Messages { get; }
        
        /// <inheritdoc />
        public ITokenStream<TToken> Remainder { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Expectations { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        /// <param name="successful">Whether the operation completed successfully.</param>
        /// <param name="value">The result of the operation.</param>
        /// <param name="messages">The messages produced by the operation.</param>
        /// <param name="remainder">The remaining tokens.</param>
        /// <param name="expectations">The names of the things expected.</param>
        internal ParseResult(bool successful, [CanBeNull] T value, IEnumerable<IMessage> messages, ITokenStream<TToken> remainder, IEnumerable<String> expectations)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            #endregion

            this.Successful = successful;
            this.Value = value;
            this.Messages = messages.ToList();
            this.Remainder = remainder;
            this.Expectations = expectations.ToList();
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public bool Equals(ParseResult<T, TToken> other)
        {
            return !Object.ReferenceEquals(other, null)
                && this.Successful == other.Successful
                && Object.Equals(this.Value, other.Value)
                && MultiSetComparer<IMessage>.Default.Equals(this.Messages, other.Messages)
                && Object.Equals(this.Remainder, other.Remainder)
                && MultiSetComparer<String>.Default.Equals(this.Expectations, other.Expectations);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.Successful.GetHashCode();
                hash = hash * 29 + this.Value?.GetHashCode() ?? 0;
                hash = hash * 29 + MultiSetComparer<IMessage>.Default.GetHashCode(this.Messages);
                hash = hash * 29 + this.Remainder.GetHashCode();
                hash = hash * 29 + MultiSetComparer<String>.Default.GetHashCode(this.Expectations);
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
            var resultStr = this.Successful ? $"success: {this.Value}" : "failed";
            ;
            return $"{resultStr} {{{this.Remainder}}}";
        }
    }

    /// <summary>
    /// Functions for working with <see cref="IParseResult{T, TToken}"/> objects.
    /// </summary>
    public static class ParseResult
    {
        /// <summary>
        /// Creates a successful parse result with the specified value and remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="remainder">The remaining tokens.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> Success<T, TToken>(T value, ITokenStream<TToken> remainder)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            #endregion

            return new ParseResult<T, TToken>(
                true,
                value,
                Collection.Empty<IMessage>(),
                remainder,
                Collection.Empty<String>()
                );
        }

        /// <summary>
        /// Creates a failed parse result with the specified remaining tokens.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="remainder">The remaining tokens.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> Fail<T, TToken>(ITokenStream<TToken> remainder)
        {
            #region Contract
            if (remainder == null)
                throw new ArgumentNullException(nameof(remainder));
            #endregion

            return new ParseResult<T, TToken>(
                false,
                default(T),
                Collection.Empty<IMessage>(),
                remainder,
                Collection.Empty<String>()
                );
        }

        /// <summary>
        /// Creates a parse result with the specified message added to it.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="message">The message to add; or <see langword="null"/>.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> WithMessage<T, TToken>(this IParseResult<T, TToken> result, [CanBeNull] IMessage message)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            #endregion

            return result.WithMessages(new[] { message });
        }

        /// <summary>
        /// Creates a parse result with the specified messages added to it.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="messages">The messages to add.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> WithMessages<T, TToken>(this IParseResult<T, TToken> result, IEnumerable<IMessage> messages)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new ParseResult<T, TToken>(
                result.Successful,
                result.Value,
                result.Messages.Concat(messages.Where(m => m != null)),
                result.Remainder,
                result.Expectations
                );
        }

        /// <summary>
        /// Creates a parse result with the specified expectation added to it.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="expectation">The expectation to add; or <see langword="null"/>.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> WithExpectation<T, TToken>(this IParseResult<T, TToken> result, [CanBeNull] string expectation)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            #endregion

            return result.WithExpectations(new[] { expectation });
        }

        /// <summary>
        /// Creates a parse result with the specified expectations added to it.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="expectations">The expectations to add.</param>
        /// <returns>The parse result.</returns>
        public static IParseResult<T, TToken> WithExpectations<T, TToken>(this IParseResult<T, TToken> result, IEnumerable<String> expectations)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (expectations == null)
                throw new ArgumentNullException(nameof(expectations));
            #endregion

            return new ParseResult<T, TToken>(
                result.Successful,
                result.Value,
                result.Messages,
                result.Remainder,
                result.Expectations.Union(expectations.Where(e => e != null))
                );
        }

        /// <summary>
        /// Creates a parse result by applying a function to a successful result,
        /// and passing through a failed result.
        /// </summary>
        /// <typeparam name="TIn">The type of the input result value.</typeparam>
        /// <typeparam name="TOut">The type of the output result value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="next">The function to apply when the result is successful.</param>
        /// <returns>The result returned by <paramref name="next"/>; or the failed result.</returns>
        public static IParseResult<TOut, TToken> OnSuccess<TIn, TOut, TToken>(this IParseResult<TIn, TToken> result, Func<IParseResult<TIn, TToken>, IParseResult<TOut, TToken>> next)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            #endregion

            if (!result.Successful)
            {
                return ParseResult.Fail<TOut, TToken>(result.Remainder)
                    .WithMessages(result.Messages)
                    .WithExpectations(result.Expectations);
            }
            else
            {
                return next(result);
            }
        }

        /// <summary>
        /// Creates a parse result by applying a function to a failed result,
        /// and passing through a successful result.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="result">The parse result to modify.</param>
        /// <param name="next">The function to apply when the result is successful.</param>
        /// <returns>The successful result, or the result returned by <paramref name="next"/>.</returns>
        public static IParseResult<T, TToken> OnFailure<T, TToken>(this IParseResult<T, TToken> result, Func<IParseResult<T, TToken>, IParseResult<T, TToken>> next)
        {
            #region Contract
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            #endregion

            if (result.Successful)
                return result;
            else
                return next(result);
        }

        /// <summary>
        /// Returns either the first result when it failed while consuming more than the second or was successful,
        /// the second result when it failed while consuming more than the first or was successful,
        /// or a combination of both when both failed and consumed equally much.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <param name="first">The first result.</param>
        /// <param name="second">The second result.</param>
        /// <returns>The result.</returns>
        internal static IParseResult<T, TToken> Or<T, TToken>(this IParseResult<T, TToken> first, IParseResult<T, TToken> second)
        {
            if (first.Successful)
                return first;
            else if (second.Successful)
                return second;
            else if (first.Remainder.Count < second.Remainder.Count)
                return first;
            else if (first.Remainder.Count > second.Remainder.Count)
                return second;
            else
            {
                return ParseResult.Fail<T, TToken>(first.Remainder)
                    .WithMessages(first.Messages)
                    .WithMessages(second.Messages)
                    .WithExpectations(first.Expectations)
                    .WithExpectations(second.Expectations);
            }
        }
    }
}
