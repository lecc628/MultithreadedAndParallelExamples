using System;
using System.Threading;
using System.Threading.Tasks;

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
        // Generating 100,000 random numbers with a midpoint value. Please, wait...
        // Thread_2:
        //    Random point values: 32,589,098
        //    Random midpoint values: 32,432 (0.100%)
        // Thread_0:
        //    Random point values: 32,664,437
        //    Random midpoint values: 32,516 (0.100%)
        // Thread_1:
        //    Random point values: 34,873,722
        //    Random midpoint values: 35,052 (0.101%)
        //
        // Total random point values: 100,127,257
        // Total random midpoint values: 100,000 (0.100%)
        public static void Main_01()
        {
            try
            {
                // Do not synchronize access to midpointsCount because at this point only exits one thread (the main thread).
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

    // The following example is similar to the previous one, except that it uses the Task class
    // instead of the Thread class to generate random midpoint integers. In this example,
    // a lambda expression replaces the GenerateNumbers thread procedure, and the call to
    // the Task.WhenAll method eliminates the need for the CountdownEvent object.
    public class InterlockedExample_WithTask
    {
        private const int LOWER_BOUND = 0;
        private const int UPPER_BOUND = 1001;
        private const int MIDPOINT = (UPPER_BOUND - LOWER_BOUND) / 2;
        private const int TASKS_COUNT = 3;

        private static int totalPoints = 0;
        private static int totalMidpoints = 0;
        private static int midpointsCount = 100000;

        private static readonly Random rnd = new();
        private static readonly object rndLock = new();

        // The example displays output like the following:
        // Generating 100,000 random numbers with a midpoint value. Please, wait...
        // Task 10:
        //    Random point values: 34,865,418
        //    Random midpoint values: 34,757 (0.100%)
        // Task 8:
        //    Random point values: 33,013,420
        //    Random midpoint values: 33,000 (0.100%)
        // Task 9:
        //    Random point values: 32,226,099
        //    Random midpoint values: 32,243 (0.100%)
        //
        //Total random point values: 100,104,937
        //Total random midpoint values: 100,000 (0.100%)
        public static async Task Main/*_02*/()
        {
            // Do not synchronize access to midpointsCount because at this point only exits one thread (the main thread).
            Console.WriteLine($"Generating {midpointsCount:N0} random numbers with a midpoint value. Please, wait...");

            Task[] tasks = new Task[TASKS_COUNT];

            for (int i = 0; i < TASKS_COUNT; ++i)
            {
                tasks[i] = Task.Run(
                    () =>
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

                        string s = string.Format("Task {0}:\n", Task.CurrentId)
                            + string.Format("   Random point values: {0:N0}\n",
                                totalPointsLocal)
                            + string.Format("   Random midpoint values: {0:N0} ({1:P3})",
                                totalMidpointsLocal, totalMidpointsLocal / ((double)totalPointsLocal));
                        Console.WriteLine(s);
                    }
                );
            }

            await Task.WhenAll(tasks);

            Console.WriteLine();
            Console.WriteLine("Total random point values: {0:N0}",
                totalPoints);
            Console.WriteLine("Total random midpoint values: {0:N0} ({1:P3})",
                totalMidpoints, totalMidpoints / ((double)totalPoints));
        }
    }
}
