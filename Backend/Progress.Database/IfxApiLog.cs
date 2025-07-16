using System;
using System.Collections.Generic;

namespace Progress.Database;

public partial class IfxApiLog
{
    public long Id { get; set; }

    public int? UzId { get; set; }

    public DateTime Date { get; set; }

    public int Type { get; set; }

    public string? Message { get; set; }

    public string? InnerException { get; set; }

    public string? StackTrace { get; set; }

    public string? XmlRequest { get; set; }

    public string? XmlResponse { get; set; }

    public long? ConsumedTime { get; set; }
}
