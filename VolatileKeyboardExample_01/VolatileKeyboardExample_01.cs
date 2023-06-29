using System;
using System.Threading;

namespace VolatileExamples
{
    public class Worker
    {
        // This method is called when the thread is started.
        public void DoWork()
        {
            bool work = false;

            while (!_shouldStop)
            {
                // Simulate some work.
                work = !work;
            }

            Console.WriteLine("Worker thread: terminating gracefully.");
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        // Keyboard volatile is used as a hint to the compiler to indicate
        // that this field is accessed by multiple threads.
        private volatile bool _shouldStop;
    }

    public class WorkerThreadTest
    {
        // With the volatile modifier added to the declaration of _shouldStop in place,
        // you'll always get the same results. However, without that modifier on
        // the _shouldStop member, the behavior is unpredictable. The DoWork method
        // may optimize the member access, resulting in reading stale data. Because of
        // the nature of multi-threaded programming, the number of stale reads is
        // unpredictable. Different runs of the program will produce somewhat different
        // results.

        public static void Main()
        {
            // Create the worker thread object. This does not start the thread.
            Worker worker = new();
            Thread workerThread = new(worker.DoWork);

            // Start the worker thread.
            workerThread.Start();
            Console.WriteLine("Main thread: starting worker thread...");

            // Loop until the worker thread activates.
            while (!workerThread.IsAlive) ;

            // Put the main thread to sleep for 1s to allow the worker thread to do
            // some work.
            Thread.Sleep(4000);

            // Request that the worker thread stop itself.
            worker.RequestStop();

            // Use the Thread.Join() method to block the current thread until
            // the object's thread terminates.
            workerThread.Join();
            Console.WriteLine("Main thread: worker thread has terminated.");
        }
    }

    // Sample output:
    // Main thread: starting worker thread...
    // Worker thread: terminating gracefully.
    // Main thread: worker thread has terminated.
}
