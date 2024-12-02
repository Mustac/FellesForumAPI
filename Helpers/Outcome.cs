namespace FellesForumAPI.Helpers
{
    /// <summary>
    /// Represents the outcome of an operation, including success status and an optional message.
    /// </summary>
    public class Outcome
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets a message providing additional context or information about the outcome.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Outcome"/> class.
        /// </summary>
        /// <param name="isSuccessful">Indicates whether the operation was successful.</param>
        /// <param name="message">An optional message providing additional context or information.</param>
        public Outcome(bool isSuccessful, string message = "")
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

    /// <summary>
    /// Represents the outcome of an operation, including success status, an optional message, and optional data.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the outcome.</typeparam>
    public class Outcome<T> : Outcome
    {
        /// <summary>
        /// Gets the data returned from the operation, if any.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Outcome{T}"/> class with a success status, optional data, and an optional message.
        /// </summary>
        /// <param name="isSuccessful">Indicates whether the operation was successful.</param>
        /// <param name="data">The data returned from the operation, if any.</param>
        /// <param name="message">An optional message providing additional context or information.</param>
        public Outcome(bool isSuccessful, T data = default, string message = "")
            : base(isSuccessful, message)
        {
            Data = data;
        }
    }
}
