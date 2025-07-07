namespace Progress.Domain.Model;

public class CustomerShort
{
	public int Id { get; set; }
	public string Kod { get; set; } = string.Empty;
	public string Nazwa { get; set; } = string.Empty;
	public string NIP { get; set; } = string.Empty;
	public Addres Adres { get; set; } = new Addres();
}
