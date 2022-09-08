using System;
using System.Threading;

namespace ThreadExamples
{
    // The current thread (in this case the main thread) calls Join() method
    // for the second thread (ThreadProc), then the current thread waits (is blocked)
    // until the second thread ends.
    public class ThreadExample_03
    {
        private const int MAIN_THREAD_ITERATIONS_COUNT = 50;
        private const int THREAD_PROC_ITERATIONS_COUNT = 200;

        public static void Main()
        {
            Console.WriteLine("Main thread: Start the second thread ThreadProc.");

            // The constructor for the Thread class requires a ThreadStart
            // delegate that represents the method to be executed on the
            // thread. C# simplifies the creation of this delegate.
            var thread = new Thread(ThreadProc);

            // Start ThreadProc. Note that on a uniprocessor, the new
            // thread does not get any processor time until the main thread
            // is preempted or yields. Uncomment the Thread.Sleep that
            // follows thread.Start() to see the difference.
            thread.Start();
            //Thread.Sleep(0);

            for (var i = 0; i < MAIN_THREAD_ITERATIONS_COUNT; ++i)
            {
                Console.WriteLine($"Main thread: Do some work {i}");
                // Yield the rest of the time slice.
                Thread.Sleep(0);
            }

            Console.WriteLine("Main thread: Call Join() for second thread, then main thread waits (is blocked) until second thread ThreadProc ends.");
            thread.Join();
            Console.WriteLine("Main thread: ThreadProc.Join() has returned. Press Enter to end program.");
            Console.ReadLine();
        }

        // Simple threading scenario: Start this static method running
        // on a second thread.
        // The ThreadProc method is called when the thread starts.
        // It loops several times, writing to the console and yielding
        // the rest of its time slice each time, and then ends.
        private static void ThreadProc()
        {
            for (var i = 0; i < THREAD_PROC_ITERATIONS_COUNT; ++i)
            {
                Console.WriteLine($"ThreadProc: {i}");
                // Yield the rest of the time slice.
                Thread.Sleep(0);
            }
        }
    }
}
