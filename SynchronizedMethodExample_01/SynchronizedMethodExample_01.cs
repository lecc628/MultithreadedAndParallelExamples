using System;
using System.Collections;

namespace SynchronizedMethodExamples
{
    // Different collections as ArrayList and Hashtable are not synchronized (thread-safe)
    // by default. So if you want to get a synchronized version of them, you can use its
    // static methods ArrayList ArrayList.Sinchronized(ArrayList arrayList) and
    // Hashtable Hashtable.Sinchronized(Hashtable hashtable) respectively.
    // These methods return a wrapper to the specified collection that is synchronized
    // (thread-safe). Synchronized() method is a thread-safe for multiples readers and
    // writers; it guarantees that there is only one writer writing at a time.
    //
    // Note
    //
    //   * Enumerating through a collection is intrinsically not a thread-safe procedure.
    //   Even when a collection is synchronized (thread-safe), other threads can still modify
    //   the collection, which causes the enumerator to throw an exception. To guarantee
    //   thread safety during enumeration, you can either lock the collection during the entire
    //   enumeration or catch the exceptions resulting from changes made by other threads.
    //
    //   * You must NOT use these collections. You must use the collections
    //   of namespace System.Collections.Concurrent.
    public class SynchronizedMethodExample_01
    {
        public static void Main()
        {
            ArrayList arrayList = new()
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            ArrayList arrayListSync = ArrayList.Synchronized(arrayList);

            Console.WriteLine($"\"{nameof(arrayList)}\" is {GetSynchronizedText(arrayList.IsSynchronized)}.");
            Console.WriteLine($"\"{nameof(arrayListSync)}\" is {GetSynchronizedText(arrayListSync.IsSynchronized)}.");

            Hashtable hashtable = new()
            {
                [0] = "AA",
                [1] = "BB",
                [2] = "CC",
                [3] = "DD",
                [4] = "EE"
            };

            Hashtable hashtableSync = Hashtable.Synchronized(hashtable);

            Console.WriteLine();
            Console.WriteLine($"\"{nameof(hashtable)}\" is {GetSynchronizedText(hashtable.IsSynchronized)}.");
            Console.WriteLine($"\"{nameof(hashtableSync)}\" is {GetSynchronizedText(hashtableSync.IsSynchronized)}.");
        }

        private static string GetSynchronizedText(bool isSynchronized) =>
            isSynchronized ? "synchronized" : "not synchronized";
    }
}
