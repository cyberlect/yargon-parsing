using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Virtlink.Utilib.Collections;

namespace Yargon.Parsing
{
    /// <summary>
    /// A result.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    public sealed class Result<T> : IResult<T>
    {
        /// <inheritdoc />
        public bool Successful { get; }

        private readonly T value;

        /// <inheritdoc />
        [CanBeNull]
        public T Value
        {
            get
            {
                #region Contract
                if (!this.Successful)
                    throw new InvalidOperationException("No value available.");
                #endregion

                return this.value;
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<String> Messages { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="successful">Whether the operation completed successfully.</param>
        /// <param name="value">The result of the operation.</param>
        /// <param name="messages">The messages produced by the operation.</param>
        public Result(bool successful, [CanBeNull] T value, IReadOnlyCollection<String> messages)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            this.Successful = successful;
            this.value = value;
            this.Messages = messages;
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public bool Equals(Result<T> other)
        {
            return !Object.ReferenceEquals(other, null)
                && this.Successful == other.Successful
                && Object.Equals(this.Value, other.Value)
                && MultiSetComparer<String>.Default.Equals(this.Messages, other.Messages);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.Successful.GetHashCode();
                hash = hash * 29 + this.Value?.GetHashCode() ?? 0;
                hash = hash * 29 + MultiSetComparer<String>.Default.GetHashCode(this.Messages);
            }
            return hash;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as Result<T>);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Result{T}"/> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Result<T> left, Result<T> right) => Object.Equals(left, right);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Result{T}"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Result<T> left, Result<T> right) => !(left == right);
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Successful ? $"success: {this.Value}" : "failed";
        }
    }

    /// <summary>
    /// Functions for working with <see cref="IResult{T}"/> objects.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The created result.</returns>
        public static IResult<T> Success<T>([CanBeNull] T value)
        {
            return Success(value, List.Empty<String>());
        }

        /// <summary>
        /// Creates a successful result with the specified value and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IResult<T> Success<T>([CanBeNull] T value, params string[] messages)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Success(value, (IReadOnlyCollection<String>)messages);
        }

        /// <summary>
        /// Creates a successful result with the specified value and messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IResult<T> Success<T>([CanBeNull] T value, IReadOnlyCollection<String> messages)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new Result<T>(true, value, messages);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The created result.</returns>
        public static IResult<T> Fail<T>()
        {
            return Fail<T>(List.Empty<String>());
        }

        /// <summary>
        /// Creates a failed result with the specified messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IResult<T> Fail<T>(params string[] messages)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return Fail<T>((IReadOnlyCollection<String>) messages);
        }

        /// <summary>
        /// Creates a failed result with the specified messages.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="messages">The messages.</param>
        /// <returns>The created result.</returns>
        public static IResult<T> Fail<T>(IReadOnlyCollection<String> messages)
        {
            #region Contract
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));
            #endregion

            return new Result<T>(false, default(T), List.Empty<String>());
        }
    }
}
