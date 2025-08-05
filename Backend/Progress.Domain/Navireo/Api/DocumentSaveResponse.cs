namespace Progress.Domain.Navireo.Api
{
  public class DocumentSaveResponse
  {
    public int DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public int PayDocumentId { get; set; }

    public string DocumentType { get; set; } = string.Empty;
  }
}
