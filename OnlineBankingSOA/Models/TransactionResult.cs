namespace OnlineBanking.Models
{
    public class TransactionResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
