namespace Progress.Domain.Model;

public class Customer
{
	public long Id { get; set; }
	public string Kod { get; set; } = string.Empty;
	public string Nazwa { get; set; } = string.Empty;
	public string NIP { get; set; } = string.Empty;
	public Addres Adres { get; set; } = new Addres();
}
