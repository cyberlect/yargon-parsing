using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// The result of a parser.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <typeparam name="TToken">The type of tokens.</typeparam>
    public interface IParseResult<out T, out TToken>
    {
        /// <summary>
        /// Gets the actual result.
        /// </summary>
        /// <value>The actual result.</value>
        IResult<T> Result { get; }

        /// <summary>
        /// Gets whether the operation successfully completed.
        /// </summary>
        /// <value><see langword="true"/> when the operation was successful;
        /// otherwise, <see langword="false"/>.</value>
        bool Successful { get; }

        /// <summary>
        /// Gets the result of the operation.
        /// </summary>
        /// <value>The result of the operation; or the default of <typeparamref name="T"/>.</value>
        T Value { get; }

        /// <summary>
        /// Gets the messages that resulted from the operation.
        /// </summary>
        /// <value>The messages.</value>
        IReadOnlyCollection<String> Messages { get; }

        /// <summary>
        /// Gets the remaining token stream after the operation completed.
        /// </summary>
        /// <value>The remaining token stream.</value>
        ITokenStream<TToken> Remainder { get; }
    }
}
