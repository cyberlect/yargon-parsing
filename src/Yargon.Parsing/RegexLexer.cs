using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Yargon.Parsing
{
    public class RegexLexer<TTokenType> : ILexer<Token<TTokenType>>
    {
        private readonly IReadOnlyList<(TTokenType, Regex)> tokenExpressions;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexLexer{TTokenType}"/> class.
        /// </summary>
        /// <param name="tokenExpressions">The token expressions, which are tuples of a token type
        /// and a corresponding regular expression. Regular expressions that occur earlier in the list
        /// are given priority of later expressions when they both match.</param>
        public RegexLexer(IReadOnlyList<(TTokenType, Regex)> tokenExpressions)
        {
            #region Contract
            if (tokenExpressions == null)
                throw new ArgumentNullException(nameof(tokenExpressions));
            #endregion

            this.tokenExpressions = tokenExpressions;
        }
        #endregion

        /// <inheritdoc />
        public ITokenStream<Token<TTokenType>> Lex(TextReader reader)
        {
            #region Contract
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            #endregion
            
            return new TokenSequence<Token<TTokenType>>(Tokenize(reader));
        }

        /// <inheritdoc />
        protected IEnumerable<Token<TTokenType>> Tokenize(TextReader reader)
        {
            var document = reader.ReadToEnd();
            var location = SourceLocation.Zero;

            int offset = 0;
            while (offset < document.Length)
            {
                var (type, match) = TryMatch(document, offset);
                if (match.Success)
                {
                    string text = match.Value;
                    var endLocation = location.AddString(text);
                    var range = new SourceRange(location, endLocation);
                    yield return BuildToken(type, text, range);
                    offset = match.Index + match.Length;
                    location = endLocation;
                }
                else
                {
                    throw new FormatException($"Illegal character: '{document[offset]}'");
                }
            }
        }

        /// <summary>
        /// Attempts to match any of the predefined regular expressions.
        /// </summary>
        /// <param name="text">The text to match on.</param>
        /// <param name="offset">The offset in the text to match.</param>
        /// <returns>The first match, or an empty match when nothing matched.</returns>
        private (TTokenType, Match) TryMatch(string text, int offset)
        {
            foreach (var (type, regex) in this.tokenExpressions)
            {
                var match = regex.Match(text, offset);
                if (match.Success)
                    return (type, match);
            }
            return (default(TTokenType), Match.Empty);
        }

        /// <summary>
        /// Builds a new token.
        /// </summary>
        /// <param name="type">The token type.</param>
        /// <param name="text">The token text.</param>
        /// <param name="range">The source range.</param>
        /// <returns>The built token.</returns>
        protected virtual Token<TTokenType> BuildToken(TTokenType type, string text, SourceRange range)
            => new Token<TTokenType>(text, type, range);
    }
}
