using Progress.Domain.Navireo;
using System.Runtime.Serialization;

namespace Progress.Navireo.Exceptions
{
    [Serializable]
    public class DocumentUpdateException : SystemException
    {
        public DocumentUpdateResult UpdateResult { get; private set; }

        public DocumentUpdateException()
        {

        }

        public DocumentUpdateException(DocumentUpdateResult result)
        {
      UpdateResult = result;
        }

        public DocumentUpdateException(DocumentUpdateResult result, string message)
            : base(message)
        {
      UpdateResult = result;
        }
        public DocumentUpdateException(DocumentUpdateResult result, string message, Exception inner)
            : base(message, inner)
        {
      UpdateResult = result;
        }

        // This constructor is needed for serialization.
        protected DocumentUpdateException(DocumentUpdateResult result, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
      UpdateResult = result;
        }
    }
}
