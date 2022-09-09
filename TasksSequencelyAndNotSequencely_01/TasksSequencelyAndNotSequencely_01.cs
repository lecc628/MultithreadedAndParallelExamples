using System;
using System.Threading.Tasks;

namespace TaskExamples
{
    public class TasksSequencelyAndNotSequencely_01
    {
        public static async Task Main()
        {
            Console.WriteLine("Executes tasks sequencely:");
            await ExecuteTasksSequencely();

            Console.WriteLine("\nExecutes tasks not sequencely:");
            await ExecuteTasksNotSequencely();

            Console.WriteLine("\nCompleted!");
        }

        public static async Task ExecuteTasksSequencely()
        {
            await Task.Run(
                async () =>
                {
                    await Task.Delay(3000);
                    Console.WriteLine("Task 1");
                }
            );

            await Task.Run(
                () =>
                {
                    Console.WriteLine("Task 2");
                }
            );

            await Task.Run(
                () =>
                {
                    Console.WriteLine("Task 3");
                }
            );
        }

        public static async Task ExecuteTasksNotSequencely()
        {
            var t1 = Task.Run(
                async () =>
                {
                    await Task.Delay(3000);
                    Console.WriteLine("Task 1");
                }
            );

            var t2 = Task.Run(
                () =>
                {
                    Console.WriteLine("Task 2");
                }
            );

            var t3 = Task.Run(
                () =>
                {
                    Console.WriteLine("Task 3");
                }
            );

            await Task.WhenAll(t1, t2, t3);
        }
    }
}
