using System;
using System.Collections.Generic;
using System.Text;
using Virtlink.Utilib;

namespace Yargon.Parsing
{
    /// <summary>
    /// A message about a source file.
    /// </summary>
    public class Message : IMessage
    {
        /// <inheritdoc />
        public MessageSeverity Severity { get; }

        /// <inheritdoc />
        public SourceRange Range { get; }

        /// <inheritdoc />
        public string Text { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="severity">The message severity.</param>
        /// <param name="text">The message text.</param>
        /// <param name="range">The source range; or an empty range.</param>
        public Message(MessageSeverity severity, string text, SourceRange range)
        {
            #region Contract
            if (!Enum.IsDefined(typeof(MessageSeverity), severity))
                throw new EnumArgumentException(nameof(severity), (int)severity, typeof(MessageSeverity));
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            #endregion

            this.Severity = severity;
            this.Text = text;
            this.Range = range;
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as Message);

        /// <inheritdoc />
        public bool Equals(Message other)
        {
            if (Object.ReferenceEquals(other, null) ||      // When 'other' is null
                other.GetType() != this.GetType())          // or of a different type
                return false;                               // they are not equal.
            return this.Severity == other.Severity
                && this.Text == other.Text
                && this.Range == other.Range;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + this.Severity.GetHashCode();
                hash = hash * 29 + this.Text.GetHashCode();
                hash = hash * 29 + this.Range.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Message"/> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Message left, Message right) => Object.Equals(left, right);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Message"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Message left, Message right) => !(left == right);
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!this.Range.IsEmpty)
            {
                sb.Append(this.Range.Start);
                sb.Append(": ");
            }
            sb.Append(this.Severity.ToString().ToLowerInvariant());
            sb.Append(": ");
            sb.Append(this.Text);

            return sb.ToString();
        }
    }
}
