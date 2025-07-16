namespace Progress.Domain.Navireo
{
    public class PaymentType : BObjectBase
    {
        public string Identity { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public decimal PriceNet { get; set; }
        public decimal PriceGross { get; set; }
        public List<Price> Prices { get; set; }
        public decimal TaxValue { get; set; }
        public Currency Currency { get; set; }
        public bool Special { get; set; }
        public Tax Tax { get; set; }
    }
}
