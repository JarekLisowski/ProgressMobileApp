namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Object base
    /// </summary>
    public class BObjectBase
    {
        /// <summary>
        /// Identyfikator
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// IsNew
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// IsUpdated
        /// </summary>
        public bool IsUpdated { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }
    }
}
