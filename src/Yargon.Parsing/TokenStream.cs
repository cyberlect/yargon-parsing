namespace Yargon.Parsing
{
    /// <summary>
    /// Functions for working with token streams.
    /// </summary>
    public static class TokenStream
    {
        /// <summary>
        /// Returns an empty token stream.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens.</typeparam>
        /// <returns>The token stream.</returns>
        public static ITokenStream<TToken> Empty<TToken>()
        {
            return TokenSequence<TToken>.Empty;
        }
    }
}
