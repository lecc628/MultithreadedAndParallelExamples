using System;

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

        public static void Main()
        {
            Console.WriteLine($"{nameof(Account)} class");
        }
    }
}
