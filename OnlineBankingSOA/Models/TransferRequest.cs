
namespace OnlineBanking.Models
{
    public class TransferRequest
    {
        public int FromAccount { get; set; }
        public int ToAccount { get; set; }
        public decimal Amount { get; set; }
    }
}
