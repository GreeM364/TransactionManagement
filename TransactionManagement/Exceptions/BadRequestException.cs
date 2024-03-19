namespace TransactionManagement.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when the client's request is invalid (HTTP 400 Bad Request).
    /// </summary>
    [Serializable]
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BadRequestException(string message) : base(message)
        { }
    }
}
