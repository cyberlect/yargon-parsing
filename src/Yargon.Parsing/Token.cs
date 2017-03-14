using System;
using JetBrains.Annotations;

namespace Yargon.Parsing
{
    /// <summary>
    /// A simple token.
    /// </summary>
    /// <typeparam name="TType">The type of token type.</typeparam>
    public struct Token<TType>
    {
        /// <summary>
        /// Gets the value of the token.
        /// </summary>
        /// <value>The token value; or <see langword="null"/>.</value>
        [CanBeNull]
        public string Value { get; }

        /// <summary>
        /// Gets the type of token.
        /// </summary>
        /// <value>The token type.</value>
        public TType Type { get; }

        /// <summary>
        /// Gets the range of the token in the source.
        /// </summary>
        /// <value>The token range; or an empty range when the token is virtual.</value>
        public SourceRange Range { get; }

        /// <summary>
        /// Gets whether the token is virtual.
        /// </summary>
        /// <value><see langword="true"/> when the token is virtual;
        /// otherwise, <see langword="false"/>.</value>
        public bool IsVirtual => this.Range.IsEmpty;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Token{TType}"/> class.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <param name="type">The token type.</param>
        /// <param name="range">The token range; or an empty range for a virtual token.</param>
        public Token([CanBeNull] string value, TType type, SourceRange range)
        {
            #region Contract
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            #endregion

            this.Value = value;
            this.Type = type;
            this.Range = range;
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Token<TType> && Equals((Token<TType>)obj);

        /// <inheritdoc />
        public bool Equals(Token<TType> other)
        {
            return Object.Equals(this.Value, other.Value)
                && Object.Equals(this.Type, other.Type)
                && this.Range == other.Range;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.Value?.GetHashCode() ?? 0;
                hash = hash * 29 + this.Type?.GetHashCode() ?? 0;
                hash = hash * 29 + this.Range.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Token{TType}"/> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Token<TType> left, Token<TType> right) => Object.Equals(left, right);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Token{TType}"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Token<TType> left, Token<TType> right) => !(left == right);
        #endregion

        /// <inheritdoc />
        public override string ToString()
            // Should not be escaped.
            => this.Value;
    }
}
