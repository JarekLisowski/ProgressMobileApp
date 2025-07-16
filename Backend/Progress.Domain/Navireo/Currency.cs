namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Waluta
    /// </summary>
    public class Currency : BObjectBase
    {
        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Kurs
        /// </summary>
        public decimal Ratio { get; set; }

        /// <summary>
        /// Data aktualizacji kursu
        /// </summary>
        public DateTime Updated { get; set; }
    }
}
