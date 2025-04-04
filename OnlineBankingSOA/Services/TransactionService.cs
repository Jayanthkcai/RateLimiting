using OnlineBanking.Models;
using System;
using System.Transactions;

namespace OnlineBanking.Services
{
    public interface ITransactionService
    {
        TransactionResult ProcessTransaction(TransactionRequest transaction);
    }

    public class TransactionService : ITransactionService
    {
        public TransactionResult ProcessTransaction(TransactionRequest transaction)
        {
            var result = new TransactionResult();

            // Validate transaction details
            if (transaction == null || transaction.Amount <= 0 || string.IsNullOrEmpty(transaction.FromAccount) || string.IsNullOrEmpty(transaction.ToAccount))
            {
                result.IsSuccess = false;
                result.Message = "Invalid transaction details.";
                result.TransactionId = Guid.NewGuid().ToString();
                return result;
            }

            // Process the transaction (e.g., debit/credit accounts)
            bool isProcessed = PerformTransaction(transaction);

            result.IsSuccess = isProcessed;
            result.Message = isProcessed ? "Transaction successful." : "Transaction failed.";
            result.TransactionId = Guid.NewGuid().ToString();
            return result;
        }

        private bool PerformTransaction(TransactionRequest transaction)
        {
            // Placeholder for actual transaction logic
            // For example, update account balances, record transaction history, etc.
            // Debit from the source account
            bool isDebited = DebitAccount(transaction.FromAccount, transaction.Amount);
            if (!isDebited)
            {
                return false;
            }

            // Credit to the destination account
            bool isCredited = CreditAccount(transaction.ToAccount, transaction.Amount);
            if (!isCredited)
            {
                // Rollback debit if credit fails
                CreditAccount(transaction.FromAccount, transaction.Amount);
                return false;
            }

            return true;
        }

        private bool DebitAccount(string account, decimal amount)
        {
            // Assuming we have a method to get the account balance
            decimal currentBalance = GetAccountBalance(account);

            // Check if the account has sufficient balance
            if (currentBalance < amount)
            {
                return false;
            }

            // Deduct the amount from the account balance
            bool isDebited = UpdateAccountBalance(account, currentBalance - amount);

            return isDebited;
        }

        private decimal GetAccountBalance(string account)
        {
            // Placeholder for getting the account balance logic
            // This should interact with the data source to retrieve the current balance
            return 1000.00m; // Example balance
        }

        private bool UpdateAccountBalance(string account, decimal newBalance)
        {
            // Placeholder for updating the account balance logic
            // This should interact with the data source to update the balance
            return true; // Assume update is successful
        }

        private bool CreditAccount(string account, decimal amount)
        {
            // Placeholder for crediting account logic
            // Return true if credit is successful, otherwise false
            return true;
        }
    }
    
}
