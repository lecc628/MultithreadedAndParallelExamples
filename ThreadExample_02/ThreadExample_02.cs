using System;
using System.Diagnostics;
using System.Threading;

namespace ThreadExamples
{
    public class ThreadExample_02
    {
        private const long DEFAULT_INTERVAL_MILLISECONDS = 5000L;

        public static void Main()
        {
            var thread = new Thread(ExecuteInForeground);
            thread.Start(7000L);

            Thread.Sleep(1000);
            Console.WriteLine("Main thread ({0}) finishes its work before second thread finishes.", Thread.CurrentThread.ManagedThreadId);
        }

        private static void ExecuteInForeground(object? intervalMilliseconds)
        {
            var interval = GetIntervalMilliseconds(intervalMilliseconds);
            var currentThread = Thread.CurrentThread;
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine(
                "Thread {0}: {1}, Priority {2}",
                currentThread.ManagedThreadId,
                currentThread.ThreadState,
                currentThread.Priority);

            do
            {
                Console.WriteLine(
                    "Thread {0}: Elapsed {1:N2} seconds",
                    currentThread.ManagedThreadId,
                    stopwatch.ElapsedMilliseconds / 1000.0);
                Thread.Sleep(500);

            } while (stopwatch.ElapsedMilliseconds <= interval);

            stopwatch.Stop();
        }

        private static long GetIntervalMilliseconds(object? intervalMilliseconds)
        {
            try
            {
                if (intervalMilliseconds is not null)
                {
                    return (long)intervalMilliseconds;
                }
            }
            catch
            { }

            return DEFAULT_INTERVAL_MILLISECONDS;
        }

        // The example displays output like the following:
        //       Thread 3: Running, Priority Normal
        //       Thread 3: Elapsed 0.00 seconds
        //       Thread 3: Elapsed 0.51 seconds
        //       Main thread (1) finishes its work before second thread finishes.
        //       Thread 3: Elapsed 1.02 seconds
        //       Thread 3: Elapsed 1.53 seconds
        //       Thread 3: Elapsed 2.05 seconds
        //       Thread 3: Elapsed 2.55 seconds
        //       Thread 3: Elapsed 3.07 seconds
        //       Thread 3: Elapsed 3.57 seconds
        //       Thread 3: Elapsed 4.07 seconds
        //       Thread 3: Elapsed 4.58 seconds
    }
}
