namespace TransactionManagement.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when a requested resource is not found (HTTP 404 Not Found).
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public NotFoundException(string message) : base(message)
        { }
    }
}
