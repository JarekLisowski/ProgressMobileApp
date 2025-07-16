namespace Progress.Navireo.Helpers
{
    public enum LogType
    {
        Exception,

        Warning,

        Message,

        BusinessUpdate,

        DocumentUpdate,

        SqliteEvent,

        TokenException,

        BusinessRead,

        DocumentRead,

        ProductRead,

        ConfigurationRead,

        FinanceUpdate,

        FinanceRead,

        DiscountSet,

        Category,

        Account,

        Common,

        RoutesRead,
    }

    public class LogItem
    {
        public long Id { get; set; }
        public LogType LogType { get; set; }
        public string Typ { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string innerException { get; set; }
        public string StackTrace { get; set; }
        public string XmlRequest { get; set; }
        public string XmlResponse { get; set; }
        public string Token { get; set; }
        public string Login { get; set; }
        public TimeSpan? ConsumedTime { get; set; }
    }
}
