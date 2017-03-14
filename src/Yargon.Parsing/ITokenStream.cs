using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// A stream of tokens.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    public interface ITokenStream<out TToken>
    {
        /// <summary>
        /// Gets whether the end of the input has been reached.
        /// </summary>
        /// <value><see langword="true"/> when the end of the input has been reached;
        /// otherwise, <see langword="false"/>.</value>
        bool AtEnd { get; }

        /// <summary>
        /// Gets the current token.
        /// </summary>
        /// <value>The current token; or the default of TToken</value>
        TToken Current { get; }

        /// <summary>
        /// Returns a token stream advanced by one token.
        /// </summary>
        /// <returns></returns>
        ITokenStream<TToken> Advance();
    }
}
