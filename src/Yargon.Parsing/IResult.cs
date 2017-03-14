using System;
using System.Collections.Generic;
using System.Text;

namespace Yargon.Parsing
{
    /// <summary>
    /// The result of an operation.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    public interface IResult<out T>
    {
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
    }
}
