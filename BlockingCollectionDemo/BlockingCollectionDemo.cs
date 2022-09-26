using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BlockingCollectionExamples
{
    public class BlockingCollectionDemo
    {
        public static async Task Main()
        {
            await AddTakeDemo.BC_AddTakeCompleteAdding();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    public class AddTakeDemo
    {
        // Demonstrates:
        //      BlockingCollection<T>.Add()
        //      BlockingCollection<T>.Take()
        //      BlockingCollection<T>.CompleteAdding()
        public static async Task BC_AddTakeCompleteAdding()
        {
            using BlockingCollection<int> bc = new();

            // Spin up a Task to populate the BlockingCollection.
            Task t1 = Task.Run(() =>
            {
                bc.Add(1);
                bc.Add(2);
                bc.Add(3);
                bc.CompleteAdding();
            });

            // Spin up a Task to consume the BlockingCollection.
            Task t2 = Task.Run(() =>
            {
                try
                {
                    // Consume consume the BlockingCollection.
                    while (true)
                    {
                        Console.WriteLine(bc.Take());
                    }
                }
                catch (InvalidOperationException)
                {
                    // An InvalidOperationException means that Take() was called on a completed collection.
                    Console.WriteLine("That's All!");
                }
            });

            await Task.WhenAll(t1, t2);
        }
    }
}
