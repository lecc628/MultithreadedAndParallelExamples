using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlockingCollectionExamples
{
    public class BlockingCollectionDemo
    {
        public static async Task Main()
        {
            Console.WriteLine($"{nameof(AddTakeDemo)}:\n");
            await AddTakeDemo.BC_AddTakeCompleteAdding();

            Console.WriteLine("\n-----------------------------------------------------------------------\n");

            Console.WriteLine($"{nameof(TryTakeDemo)}:\n");
            TryTakeDemo.BC_TryTake();

            Console.WriteLine("\n-----------------------------------------------------------------------\n");

            Console.WriteLine($"{nameof(FromToAnyDemo)}:\n");
            FromToAnyDemo.BC_FromToAny();

            Console.WriteLine("\n-----------------------------------------------------------------------\n");
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

    public class TryTakeDemo
    {
        // Demonstrates:
        //      BlockingCollection<T>.Add()
        //      BlockingCollection<T>.CompleteAdding()
        //      BlockingCollection<T>.TryTake()
        //      BlockingCollection<T>.IsCompleted
        public static void BC_TryTake()
        {
            // Construct and fill our BlockingCollection.
            using BlockingCollection<int> bc = new();
            const int NUM_ITEMS = 10000;

            for (var i = 0; i < NUM_ITEMS; ++i)
            {
                bc.Add(i);
            }
            bc.CompleteAdding();

            int outerSum = 0;

            // Delegate for consuming the BlockingCollection and adding up all items.
            Action action = () =>
            {
                int localSum = 0;

                while (bc.TryTake(out int localItem))
                {
                    localSum += localItem;
                }

                Interlocked.Add(ref outerSum, localSum);
            };

            // Launch three parallel actions to consume the BlockingCollection.
            Parallel.Invoke(action, action, action);

            Console.WriteLine("Sum[0..{0}) = {1}, should be {2}", NUM_ITEMS, outerSum, ((NUM_ITEMS * (NUM_ITEMS - 1)) / 2));
            Console.WriteLine("bc.IsCompleted = {0}, should be true", bc.IsCompleted);
        }
    }

    public class FromToAnyDemo
    {
        // Demonstrates:
        //      Bounded BlockingCollection<T>
        //      BlockingCollection<T>.TryAddToAny()
        //      BlockingCollection<T>.TryTakeFromAny()
        public static void BC_FromToAny()
        {
            BlockingCollection<int>[] bcs = new BlockingCollection<int>[2];
            bcs[0] = new BlockingCollection<int>(5);  // Collection bounded to 5 items.
            bcs[1] = new BlockingCollection<int>(5);  // Collection bounded to 5 items.

            // Should be able to add 10 items w/o blocking.
            int numFailures = 0;
            for (var i = 0; i < 10; ++i)
            {
                if (BlockingCollection<int>.TryAddToAny(bcs, i) == -1)
                {
                    ++numFailures;
                }
            }
            Console.WriteLine("TryAddToAny: {0} failures, should be 0", numFailures);

            // Should be able to retrieve 10 items.
            int numItems = 0;
            while (BlockingCollection<int>.TryTakeFromAny(bcs, out _) != -1)
            {
                ++numItems;
            }
            Console.WriteLine("TryTakeFromAny: retrieved {0} items, should be 10", numItems);

            foreach (var bc in bcs)
            {
                bc.Dispose();
            }
        }
    }
}
