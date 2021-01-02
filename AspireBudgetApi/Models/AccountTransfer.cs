﻿using System;

namespace AspireBudgetApi.Models
{
    public class AccountTransfer
    {
        public DateTime Date { get; set; }
        public double Sum { get; set; }
        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }
        public string Memo { get; set; }
        public string Cleared { get; set; }

        public static AccountTransfer FromTransactions(Transaction t1, Transaction t2, string accountTransferCategoryName)
        {
            if (Math.Abs(t1.Inflow - t2.Outflow) > 0.01 || Math.Abs(t1.Outflow - t2.Inflow) > 0.01)
            {
                throw new InvalidOperationException("Values of transactions are different");
            }

            if (t1.Category != accountTransferCategoryName || t2.Category != accountTransferCategoryName)
            {
                throw new ArgumentException($"Transactions do not have necessary category: {accountTransferCategoryName}");
            }

            return new AccountTransfer()
            {
                Date = t1.Date,
                Sum = Math.Max(t1.Inflow, t1.Outflow),
                AccountFrom = t1.Outflow > 0 ? t1.Account : t2.Account,
                AccountTo = t1.Inflow > 0 ? t1.Account : t2.Account,
                Memo = !string.IsNullOrEmpty(t1.Memo) || !string.IsNullOrEmpty(t2.Memo) ? string.Join("; ", t1.Memo.Trim(), t2.Memo.Trim()) : "",
                Cleared = t1.Cleared == t2.Cleared ? t1.Cleared : ""
            };
        }

        public static Transaction[] ToTransactions(AccountTransfer transfer, string accountTransferCategoryName)
        {
            var t1 = new Transaction
            {
                Date = transfer.Date,
                Account = transfer.AccountFrom,
                Category = accountTransferCategoryName,
                Cleared = transfer.Cleared,
                Inflow = 0,
                Outflow = transfer.Sum,
                Memo = transfer.Memo
            };
            var t2 = new Transaction
            {
                Date = transfer.Date,
                Account = transfer.AccountTo,
                Category = accountTransferCategoryName,
                Cleared = transfer.Cleared,
                Inflow = transfer.Sum,
                Outflow = 0,
                Memo = ""
            };

            return new[] { t1, t2 };
        }
    }
}
