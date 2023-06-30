using System;
using System.Net;
using System.Threading;

namespace InterlockedExamples
{
    // The following example determines how many random numbers that range from 0 to 1,000
    // are required to generate 100,000 random numbers with a midpoint value. To keep track
    // of the number of midpoint values, a variable, midpointsCount, is set equal to 100,000
    // and decremented each time the random number generator returns a midpoint value.
    // Because three threads generate the random numbers, the Interlocked.Decrement(Int32)
    // method is called to ensure that multiple threads don't update midpointsCount concurrently.
    // Note that a lock is also used to protect the random number generator, and that
    // a CountdownEvent object is used to ensure that the Main method doesn't finish execution
    // before the three threads.
    public class InterlockedExample_WithThreadAndCountdownEvent
    {
        private const int LOWER_BOUND = 0;
        private const int UPPER_BOUND = 1001;
        private const int MIDPOINT = (UPPER_BOUND - LOWER_BOUND) / 2;
        private const int THREADS_COUNT = 3;

        private static int totalPoints = 0;
        private static int totalMidpoints = 0;
        private static int midpointsCount = 100000;

        private static readonly Random rnd = new();
        private static readonly object rndLock = new();
        private static readonly CountdownEvent cde = new(1);

        // The example displays output like the following:
        // Generating 10,000 random numbers with a midpoint value. Please, wait...
        // Thread_1:
        //    Random point values: 3,421,799
        //    Random midpoint values: 3,492 (0.102%)
        // Thread_0:
        //    Random point values: 3,313,080
        //    Random midpoint values: 3,273 (0.099%)
        // Thread_2:
        //    Random point values: 3,165,538
        //    Random midpoint values: 3,235 (0.102%)
        //
        // Total random point values: 9,900,417
        // Total random midpoint values: 10,000 (0.101%)
        public static void Main()
        {
            try
            {
                // Do not synchronize midpointsCount because at this point only exits one thread (the main thread).
                Console.WriteLine($"Generating {midpointsCount:N0} random numbers with a midpoint value. Please, wait...");

                Thread thread;

                // Start three threads.
                for (int i = 0; i < THREADS_COUNT; ++i)
                {
                    cde.AddCount();

                    thread = new Thread(GenerateNumbers)
                    {
                        Name = $"Thread_{i}"
                    };
                    thread.Start();
                }

                cde.Signal();

                cde.Wait();

                Console.WriteLine();
                Console.WriteLine("Total random point values: {0:N0}",
                    totalPoints);
                Console.WriteLine("Total random midpoint values: {0:N0} ({1:P3})",
                    totalMidpoints, totalMidpoints / ((double)totalPoints));
            }
            finally
            {
                cde.Dispose();
            }
        }

        private static void GenerateNumbers()
        {
            int point;
            int totalPointsLocal = 0;
            int totalMidpointsLocal = 0;

            do
            {
                lock (rndLock)
                {
                    point = rnd.Next(LOWER_BOUND, UPPER_BOUND);
                }

                if (point == MIDPOINT)
                {
                    Interlocked.Decrement(ref midpointsCount);
                    ++totalMidpointsLocal;
                }

                ++totalPointsLocal;

            } while (Volatile.Read(ref midpointsCount) > 0);

            Interlocked.Add(ref totalPoints, totalPointsLocal);
            Interlocked.Add(ref totalMidpoints, totalMidpointsLocal);

            string s = string.Format("{0}:\n", Thread.CurrentThread.Name)
                + string.Format("   Random point values: {0:N0}\n",
                    totalPointsLocal)
                + string.Format("   Random midpoint values: {0:N0} ({1:P3})",
                    totalMidpointsLocal, totalMidpointsLocal / ((double)totalPointsLocal));
            Console.WriteLine(s);

            cde.Signal();
        }
    }
}
