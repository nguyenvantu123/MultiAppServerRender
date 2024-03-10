using System.Runtime.Serialization;

namespace BlazorWeb.Components.Pages.Authentication
{
    [Serializable]
    internal class OutdatedTokenException : Exception
    {
        public OutdatedTokenException()
        {
        }

        public OutdatedTokenException(string? message) : base(message)
        {
        }

        public OutdatedTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected OutdatedTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}