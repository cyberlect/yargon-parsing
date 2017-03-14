using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// Factory for <see cref="IMessage"/> objects.
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Creates an instance of the interface.
        /// </summary>
        /// <param name="severity">The message severity.</param>
        /// <param name="text">The message text.</param>
        /// <param name="range">The source range; or an empty range.</param>
        /// <returns>The created instance.</returns>
        IMessage Create(MessageSeverity severity, string text, SourceRange range);
    }
}
