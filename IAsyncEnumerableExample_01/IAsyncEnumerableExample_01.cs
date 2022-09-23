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

            Console.WriteLine("Using enumerable async stream:\n");
            await EnumerateAsyncStream();

            Console.WriteLine("\n-----------------------------------------------------------------------\n");

            Console.WriteLine("Using enumerable async stream extension created here:\n");
            await UseAsyncEnumerableExtensions();
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

        public static async Task EnumerateAsyncStream()
        {
            await foreach (var value in GetValuesAsyncUsingIAsyncEnumerable())
            {
                Console.WriteLine(value);
            }
        }

        private static async IAsyncEnumerable<int> GetValuesAsyncUsingIAsyncEnumerable()
        {
            for (var i = 1; i <= 4; ++i)
            {
                await Task.Delay(1000);
                yield return i;
            }
        }

        private static async Task UseAsyncEnumerableExtensions()
        {
            await foreach (var scoreByInterval in GetScoresStreamAsync().GetItemsByIntervalAsync(3))
            {
                Console.WriteLine(scoreByInterval);
            }
        }

        private static async IAsyncEnumerable<Tuple<string, int?>> GetScoresStreamAsync()
        {
            Tuple<string, int?>[] scores = {
                new Tuple<string, Nullable<int>>("Noir", 100),
                new Tuple<string, int?>("Jack", 78),
                new Tuple<string, int?>("Abbey", 92),
                new Tuple<string, int?>("Kanata", 95),
                new Tuple<string, int?>("Dave", 88),
                new Tuple<string, int?>("Sam", 91),
                new Tuple<string, int?>("Tokio", 98),
                new Tuple<string, int?>("Ed", null),
                new Tuple<string, int?>("Penelope", 82),
                new Tuple<string, int?>("Maria", 97),
                new Tuple<string, int?>("Linda", 99),
            };

            foreach (var score in scores)
            {
                await Task.Delay(1000);
                yield return score;
            }
        }
    }

    public static class AsyncEnumerableExtensions
    {
        public static async IAsyncEnumerable<T> GetItemsByIntervalAsync<T>(
            this IAsyncEnumerable<T> sourceSequenceAsync,
            int interval)
        {
            var index = 0;

            await foreach (var item in sourceSequenceAsync)
            {
                if (index++ % interval == 0)
                {
                    yield return item;
                }
            }
        }
    }
}
