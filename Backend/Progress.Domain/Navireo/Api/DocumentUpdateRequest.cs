namespace Progress.Domain.Navireo.Api
{
  public class DocumentUpdateRequest
  {
    public CommerceDocumentBase? Document { get; set; }
    public int OperatorId { get; set; }
  }
}
