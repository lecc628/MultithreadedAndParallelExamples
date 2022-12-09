using System;
using System.Threading;

namespace ThreadExamples
{
    public class ThreadExample_01
    {
        private const string NULL_STRING = "null";

        public static void Main()
        {
            // The first thread which is created inside a process
            // is called Main thread. It starts first and ends at last.
            Thread thread = Thread.CurrentThread;
            Console.WriteLine($"Thread name = {GetThreadName(thread)}\nThread priority = {thread.Priority}");
        }

        private static string GetThreadName(Thread thread) =>
            $"{(thread.Name is null ? NULL_STRING : "\"" + thread.Name + "\"")}";
    }
}
