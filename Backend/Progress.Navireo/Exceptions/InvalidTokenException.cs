
using System.Runtime.Serialization;

namespace Progress.Navireo.Exceptions
{
    [Serializable]
    public class InvalidTokenException : SystemException
    {
        public InvalidTokenException()
        {

        }
        public InvalidTokenException(string message)
            : base(message)
        {

        }
        public InvalidTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        protected InvalidTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
