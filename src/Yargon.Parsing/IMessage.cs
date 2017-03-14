using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// A message about a source file.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the severity of the message.
        /// </summary>
        /// <value>A member of the <see cref="MessageSeverity"/> enumeration.</value>
        MessageSeverity Severity { get; }

        /// <summary>
        /// Gets the range of the message.
        /// </summary>
        /// <value>A source range; or an empty range when the source range is not known.</value>
        SourceRange Range { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        /// <value>The message text.</value>
        string Text { get; }
    }
}
