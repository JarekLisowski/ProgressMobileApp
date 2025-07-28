namespace Progress.Domain.Api;

public class Document
{
  public int? Id { get; set; }
  public int CustomerId { get; set; }
  public Customer? Customer { get; set; }
  public string DocumentType { get; set; } = "";
  public string Number { get; set; } = "";
  public DocumentItem[] Items { get; set; } = [];
  public decimal CashPayment { get; set; } = 0;
  public decimal TotalNet { get; set; } = 0;
  public decimal TotalGross { get; set; } = 0;
  public decimal PaymentToBeSettled { get; set; } = 0;
  public decimal SecondPaymentAmount { get; set; } = 0;
  public int PaymentDueDays { get; set; } = 0;
  public int? SecondPaymentMethod { get; set; }
  public int? DeliveryMethod { get; set; }
  public int? PackagesNumber { get; set; }
  public string Comment { get; set; } = "";
  public int? UserId { get; set; }
  public string UserName { get; set; } = "";
  public DateTime IssueDate { get; set; } = DateTime.Today;

}
