using System.Runtime.Serialization;

namespace Progress.Navireo.Exceptions
{
    [Serializable]
    public class UserException : SystemException
    {
        public UserException()
        {

        }
        public UserException(string message)
            : base(message)
        {

        }
        public UserException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        protected UserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
