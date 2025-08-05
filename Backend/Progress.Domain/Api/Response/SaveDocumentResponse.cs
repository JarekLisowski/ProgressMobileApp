namespace Progress.Domain.Api.Response
{
  public class SaveDocumentResponse : ApiResult
  {
    public int DocumentId { get; set; }
    public int PayDocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
  }
}
