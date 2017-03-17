namespace Yargon.Parsing
{
    /// <summary>
    /// Specifies the kind of message.
    /// </summary>
    public enum MessageSeverity
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        None = 0,
        /// <summary>
        /// An informational message.
        /// </summary>
        Info,
        /// <summary>
        /// A warning message.
        /// </summary>
        Warning,
        /// <summary>
        /// An error message.
        /// </summary>
        Error,
    }
}
