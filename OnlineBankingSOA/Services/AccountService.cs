
using System.Collections.Generic;

namespace OnlineBanking.Services
{
    public interface IAccountService
    {
        decimal? GetBalance(int accountId);
        bool Transfer(int fromAccount, int toAccount, decimal amount);
    }

    public class AccountService : IAccountService
    {
        private static readonly Dictionary<int, decimal> Accounts = new Dictionary<int, decimal>
        {
            {1001, 5000.00M},
            {1002, 10000.00M},
            {1003, 7500.00M}
        };

        public decimal? GetBalance(int accountId) =>
            Accounts.TryGetValue(accountId, out var balance) ? balance : (decimal?)null;

        public bool Transfer(int fromAccount, int toAccount, decimal amount)
        {
            if (!Accounts.ContainsKey(fromAccount) || !Accounts.ContainsKey(toAccount) || Accounts[fromAccount] < amount)
                return false;

            Accounts[fromAccount] -= amount;
            Accounts[toAccount] += amount;
            return true;
        }
    }
}
