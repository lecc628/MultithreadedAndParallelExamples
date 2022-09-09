using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IAsyncEnumerableExamples
{
    public class IAsyncEnumerableExample_01
    {
        public static async Task Main()
        {
            Console.WriteLine("Using enumerable sync stream:\n");
            await EnumerateSyncStream();

            Console.WriteLine("\n-----------------------------------------------------------------------\n");
        }

        public static async Task EnumerateSyncStream()
        {
            foreach (var value in await GetValuesAsyncUsingTaskIEnumerable())
            {
                Console.WriteLine(value);
            }
        }

        private static async Task<IEnumerable<int>> GetValuesAsyncUsingTaskIEnumerable()
        {
            var list = new List<int>();

            for (var i = 1; i <= 4; ++i)
            {
                await Task.Delay(1000);
                list.Add(i);
            }

            return list;
        }
    }
}
