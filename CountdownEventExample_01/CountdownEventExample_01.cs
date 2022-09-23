using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CountdownEventExamples
{
    public class CountdownEventExample_01
    {
        public static async Task Main()
        {
            const int COUNT = 1000000;

            // Initialize a queue, and a countdown event.
            ConcurrentQueue<int> queue = new(Enumerable.Range(0, COUNT));
            CountdownEvent cde = new(COUNT);  // Initial count = COUNT
            Print(cde, "Creates queue.");

            // This is the logic for all queue consumers.
            Action action = () => {
                while (queue.TryDequeue(out _))
                {
                    // Decrement CountdownEvent count once for each element consumed from queue.
                    cde.Signal();
                }
            };

            // Now empty the queue with a couple of asynchronous tasks.
            Task consumer_1 = Task.Factory.StartNew(action);
            Task consumer_2 = Task.Factory.StartNew(action);
            //Parallel.Invoke(action);

            // And wait for queue to empty by waiting on CountdownEvent.
            cde.Wait();  // Will return when CountdownEvent count reaches zero.
            Print(cde, "Done emptying queue.");

            // Proper form is to wait for the tasks to complete, even if you know that their work
            // is done already.
            await Task.WhenAll(consumer_1, consumer_2);

            // Resetting will cause the CountdownEvent to un-set, and resets InitialCount/CurrentCount
            // to the specified value.
            cde.Reset(10);
            Print(cde, "Resets the CountdownEvent.");

            // AddCount will affect the CurrentCount, but not the InitialCount.
            cde.AddCount(2);
            Print(cde, "Increments the CountdownEvent current count by two.");

            // Now try waiting with cancellation.
            CancellationTokenSource cts = new();
            cts.Cancel();  // Communicates a request for cancellation.
            try
            {
                //foreach (var item in Enumerable.Range(0, 12)) { queue.Enqueue(item); }
                //Task consumer_3 = Task.Factory.StartNew(action);

                cde.Wait(cts.Token);

                //await consumer_3;
                //Print(cde, "Done emptying queue.");
            }
            catch (OperationCanceledException)
            {
                Print(cde, "cde.Wait(preCanceledToken) threw OperationCanceledException, as expected.");
            }
            finally
            {
                cts.Dispose();
            }

            // Release a CountdownEvent when you are done with it.
            cde.Dispose();
        }

        private static void Print(CountdownEvent cde, string prefixMessage) =>
            Console.WriteLine(
                "{0}  InitialCount = {1}, CurrentCount = {2}, IsSet = {3}",
                prefixMessage,
                cde.InitialCount,
                cde.CurrentCount,
                cde.IsSet);

        // The example displays the following output:
        // Creates queue.  InitialCount = 1000000, CurrentCount = 1000000, IsSet = False
        // Done emptying queue.  InitialCount = 1000000, CurrentCount = 0, IsSet = True
        // Resets the CountdownEvent.  InitialCount = 10, CurrentCount = 10, IsSet = False
        // Increments the CountdownEvent current count by two.  InitialCount = 10, CurrentCount = 12, IsSet = False
        // cde.Wait(preCanceledToken) threw OperationCanceledException, as expected.  InitialCount = 10, CurrentCount = 12, IsSet = False
    }
}
