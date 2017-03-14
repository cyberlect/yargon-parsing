using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// Factory for <see cref="Message"/> objects.
    /// </summary>
    public sealed class MessageFactory : IMessageFactory
    {
        /// <inheritdoc />
        public IMessage Create(MessageSeverity severity, string text, SourceRange range)
            => new Message(severity, text, range);
    }
}
