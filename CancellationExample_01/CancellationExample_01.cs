using System;
using System.Threading;

namespace CancellationExamples
{
    /// <summary>
    /// Cancellation in managed threads.
    /// </summary>
    public class CancellationExample_01
    {
        public static void Main()
        {
            // Create the token source. It is disposed implicitly.
            using CancellationTokenSource cts = new();

            // Pass the token to the cancelable operation.
            ThreadPool.QueueUserWorkItem(DoSomeWork, cts.Token, true);
            Thread.Sleep(2500);

            // Request cancellation.
            cts.Cancel();
            Console.WriteLine("Cancellation set in token source.");
            Thread.Sleep(2500);

            // Cancellation should have happen. However you can verify it
            // through implementing a mechanism that catches OperationCanceledException
            // that you throw in the listening methods using the token.ThrowIfCancellationRequested()
            // method.
        }

        /// <summary>
        /// Some work that executes in another thread. It is a cancelable operation.
        /// </summary>
        /// <param name="token">Token used to do listening of cancellation.</param>
        private static void DoSomeWork(CancellationToken token)
        {
            for (var i = 0; i < 100; ++i)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"In iteration {i + 1}, cancellation has been requested.");
                    // Perform cleanup if necessary.
                    //...

                    // Terminate the operation.
                    return;
                }

                // Simulate some work.
                Thread.Sleep(100);
            }
        }
    }

    // The example displays output like the following:
    //       Cancellation set in token source.
    //       In iteration 117, cancellation has been requested.
}
