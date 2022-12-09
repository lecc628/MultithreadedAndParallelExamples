using System;
using System.Threading.Tasks;

namespace LockExamples
{
    // The following example defines an Account class that synchronizes access to
    // its private balance field by locking on a dedicated balanceLock instance.
    // Using the same instance for locking ensures that the balance field can't be
    // updated simultaneously by two threads attempting to call the Debit or Credit
    // methods simultaneously. This class is a thread-safe class.
    public class Account
    {
        private decimal balance;
        private readonly object balanceLock = new();

        public Account(decimal initialBalance) => balance = initialBalance;

        public decimal Debit(decimal amount)
        {
            if (amount > 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The debit amount can not be positive.");
            }

            decimal appliedAmount = 0m;

            lock (balanceLock)
            {
                if (balance >= amount)
                {
                    balance += amount;
                    appliedAmount = amount;
                }
            }

            return appliedAmount;
        }

        public void Credit(decimal amount)
        {
            if (amount < 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The credit amount can not be negative.");
            }

            lock (balanceLock)
            {
                balance += amount;
            }
        }

        public decimal GetBalance()
        {
            lock (balanceLock)
            {
                return balance;
            }
        }
    }

    internal class AccountTest
    {
        // If you do not used the lock statement, the output is wrong and prints something like:
        // Balance before operation is:    1000
        // Balance after operation is:     1988
        // But if you use the lock statement (what is the correct way), the output is right and prints:
        // Balance before operation is:    1000
        // Balance after operation is:     2000
        public static async Task Main()
        {
            Account account = new(1000m);
            var tasks = new Task[100];

            PrintBalance(account.GetBalance(), "Balance before operation is:");

            for (var i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = Task.Run(() => Update(account));
            }

            await Task.WhenAll(tasks);

            PrintBalance(account.GetBalance(), "Balance after operation is:");
        }

        public static void PrintBalance(decimal balance, string prefixMessage) =>
            Console.WriteLine($"{prefixMessage}\t{balance}");

        private static void Update(Account account)
        {
            decimal[] amounts = { 0m, 2m, -3m, 6m, -2m, -1m, 8m, -5m, 11m, -6m };

            foreach (var amount in amounts)
            {
                if (amount >= 0m)
                {
                    account.Credit(amount);
                }
                else
                {
                    account.Debit(amount);
                }
            }
        }
    }
}
