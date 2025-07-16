namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Vat
    /// </summary>
    public class Tax : BObjectBase
    {
        /// <summary>
        /// Wartość procentowa 
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }
    }
}
