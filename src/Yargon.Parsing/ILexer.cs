using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// A lexer.
    /// </summary>
    /// <typeparam name="TToken">The type of tokens produced by the lexer.</typeparam>
    /// <remarks>
    /// The lexer takes a text and produces a stream of tokens that describe the text.
    /// This can be used by a parser to produce a parse tree.
    /// </remarks>
    public interface ILexer<out TToken>
    {
        /// <summary>
        /// Lexes the text from the specified text reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>The token stream.</returns>
        ITokenStream<TToken> Lex(TextReader reader);
    }
}
