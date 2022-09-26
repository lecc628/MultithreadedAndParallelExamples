using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CountdownEventExamples
{
    public class CancelableCountdownEvent
    {
        private class Data
        {
            public Data() : this(0)
            { }

            public Data(int num)
            {
                Num = num;
            }

            public int Num { get; set; }
        }

        private class DataWithToken
        {
            public DataWithToken(Data data, CancellationToken token)
            {
                Data = data;
                Token = token;
            }

            public Data Data { get; private set; }

            public CancellationToken Token { get; set; }
        }

        private static IEnumerable<Data> GetData() =>
            new List<Data> { new Data(1), new Data(2), new Data(3), new Data(4), new Data(5) };

        public static void Main()
        {
            EventWithCancellation();

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void EventWithCancellation()
        {
            IEnumerable<Data> dataSource = GetData();
            CancellationTokenSource cts = new();
            CancellationToken token = cts.Token;

            Console.WriteLine("-Press c key if you want to cancel the tasks.-");

            // Enable cancellation request from a UI thread.
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (Console.ReadKey().KeyChar == 'c')
                    {
                        Console.WriteLine();
                        cts.Cancel();
                    }
                }
            });

            // The event must have a count of at least one.
            CountdownEvent cde = new(1);

            DataWithToken item;
            // Fork work.
            foreach (Data data in dataSource)
            {
                item = new(data, token);
                // Dynamically increment the signal count.
                cde.AddCount();

                ThreadPool.QueueUserWorkItem(
                    (item) =>
                    {
                        ProcessData(item);

                        if (!token.IsCancellationRequested)
                        {
                            cde.Signal();
                        }
                    },
                    item,
                    false);
            }

            // Decrement the event by the one that was specified when creating it.
            cde.Signal();

            // The first element could be run on this thread.

            // Join work or catch cancellation.
            try
            {
                cde.Wait(token);
                Console.WriteLine("Work finished.");
            }
            catch (OperationCanceledException oce)
            {
                if (oce.CancellationToken == token)
                {
                    Console.WriteLine("User canceled.");
                }
                else
                {
                    Console.WriteLine("We don't know who canceled us.");
                    throw;
                }
            }
            finally
            {
                cde.Dispose();
                cts.Dispose();
            }
        }

        private static void ProcessData(DataWithToken dataWithToken)
        {
            if (dataWithToken.Token.IsCancellationRequested)
            {
                Console.WriteLine("Canceled before starting task = {0}.", dataWithToken.Data.Num);
                return;
            }

            for (int i = 0; i < 50; ++i)
            {
                if (dataWithToken.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancelling while executing task = {0}, iteration = {1}.", dataWithToken.Data.Num, i);
                    return;
                }

                // Simulate some work.
                Thread.Sleep(100);
            }

            Console.WriteLine("Processed task = {0}.", dataWithToken.Data.Num);
        }
    }
}
