using System;
using System.Threading;
using System.Xml.Linq;

namespace MutexExamples
{
    public class MutexExample_01
    {
        // Create a new Mutex. The creating thread does not own the mutex.
        private static readonly Mutex mutex = new();
        private const int THREADS_COUNT = 3;
        private const int ITERATIONS_COUNT = 3;

        public static void Main()
        {
            // Create the threads that will use the protected resource.
            for (var i = 0; i < THREADS_COUNT; ++i)
            {
                Thread thread = new(ThreadProc)
                {
                    Name = $"Thread_{i + 1}"
                };
                thread.Start();
            }

            // The main thread exits, but the application continues to run
            // until all foreground threads have exited.
        }

        private static void ThreadProc()
        {
            for (var i = 0; i < ITERATIONS_COUNT; ++i)
            {
                UseResource();
            }
        }

        // This method contains a resource that must be synchronized
        // so that only one thread at a time can use it.
        private static void UseResource()
        {
            var threadName = Thread.CurrentThread.Name;

            Console.WriteLine($"{threadName} is requesting the mutex.");
            // Wait until it is safe to enter.
            mutex.WaitOne();

            Console.WriteLine($"{threadName} has entered the protected area.");

            // Simulate some work.
            Thread.Sleep(3000);

            Console.WriteLine($"{threadName} is exiting the protected area.");
            // Release the mutex.
            mutex.ReleaseMutex();

            Console.WriteLine($"{threadName} has exited the mutex.");
        }
    }

    // The example displays output like the following:
    //      Thread_1 is requesting the mutex.
    //      Thread_2 is requesting the mutex.
    //      Thread_3 is requesting the mutex.
    //      Thread_1 has entered the protected area.
    //      Thread_1 is exiting the protected area.
    //      Thread_1 has exited the mutex.
    //      Thread_1 is requesting the mutex.
    //      Thread_2 has entered the protected area.
    //      Thread_2 is exiting the protected area.
    //      Thread_2 has exited the mutex.
    //      Thread_2 is requesting the mutex.
    //      Thread_3 has entered the protected area.
    //      Thread_3 is exiting the protected area.
    //      Thread_3 has exited the mutex.
    //      Thread_3 is requesting the mutex.
    //      Thread_1 has entered the protected area.
    //      Thread_1 is exiting the protected area.
    //      Thread_2 has entered the protected area.
    //      Thread_1 has exited the mutex.
    //      Thread_1 is requesting the mutex.
    //      Thread_2 is exiting the protected area.
    //      Thread_2 has exited the mutex.
    //      Thread_2 is requesting the mutex.
    //      Thread_3 has entered the protected area.
    //      Thread_3 is exiting the protected area.
    //      Thread_3 has exited the mutex.
    //      Thread_3 is requesting the mutex.
    //      Thread_1 has entered the protected area.
    //      Thread_1 is exiting the protected area.
    //      Thread_1 has exited the mutex.
    //      Thread_2 has entered the protected area.
    //      Thread_2 is exiting the protected area.
    //      Thread_3 has entered the protected area.
    //      Thread_2 has exited the mutex.
    //      Thread_3 is exiting the protected area.
    //      Thread_3 has exited the mutex.
}
