using System;
using System.Collections.Generic;

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
        /// Gets whether the operation successfully completed.
        /// </summary>
        /// <value><see langword="true"/> when the operation was successful;
        /// otherwise, <see langword="false"/>.</value>
        bool Successful { get; }

        /// <summary>
        /// Gets the result of the operation after the operation successfully completed.
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
        /// <remarks>
        /// This is the stream of tokens after the operation succeeded or failed.
        /// </remarks>
        ITokenStream<TToken> Remainder { get; }

        /// <summary>
        /// Gets the names of the things that were expected by the parser,
        /// whether it succeeded or failed.
        /// </summary>
        /// <value>A collection of expectations.</value>
        IReadOnlyCollection<String> Expectations { get; }
    }
}
