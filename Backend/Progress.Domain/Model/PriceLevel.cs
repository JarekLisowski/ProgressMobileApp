namespace Progress.Domain.Model
{
    public class PriceLevel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Primary { get; set; }

        public bool Checked { get; set; }
    }
}
