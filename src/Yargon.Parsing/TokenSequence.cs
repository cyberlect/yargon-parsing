using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// A list of tokens.
    /// </summary>
    public sealed class TokenSequence<T> : ITokenStream<T>, IEnumerable<T>
    {
        /// <summary>
        /// The inner list.
        /// </summary>
        private readonly IReadOnlyList<T> innerList;

        /// <summary>
        /// The offset within the list.
        /// </summary>
        private readonly int offset;

        /// <inheritdoc />
        public bool AtEnd => this.offset >= this.innerList.Count;

        /// <inheritdoc />
        public T Current => this.AtEnd ? default(T) : this.innerList[this.offset];

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSequence{T}"/> class.
        /// </summary>
        /// <param name="sequence">The sequence to wrap.</param>
        public TokenSequence(IEnumerable<T> sequence)
            : this(sequence.ToArray(), 0)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSequence{T}"/> class.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <param name="offset">The offset within the list where this stream starts.</param>
        private TokenSequence(IReadOnlyList<T> list, int offset)
        {
            #region Contract
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (offset < 0 || offset > list.Count)
                throw new ArgumentOutOfRangeException(nameof(offset));
            #endregion

            this.innerList = list;
            this.offset = offset;
        }
        #endregion

        /// <inheritdoc />
        public ITokenStream<T> Advance()
        {
            if (this.AtEnd)
                return this;
            return new TokenSequence<T>(this.innerList, this.offset + 1);
        }

        #region Equality
        /// <inheritdoc />
        public bool Equals(TokenSequence<T> other)
        {
            return !Object.ReferenceEquals(other, null)
                && Object.ReferenceEquals(this.innerList, other.innerList)
                && this.offset == other.offset;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.innerList.GetHashCode();
                hash = hash * 29 + this.offset.GetHashCode();
            }
            return hash;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as TokenSequence<T>);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="TokenSequence{T}"/> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(TokenSequence<T> left, TokenSequence<T> right) => Object.Equals(left, right);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="TokenSequence{T}"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(TokenSequence<T> left, TokenSequence<T> right) => !(left == right);
        #endregion

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = this.offset; i < this.innerList.Count; i++)
            {
                yield return this.innerList[i];
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Join(" ", this);
        }
    }
}
