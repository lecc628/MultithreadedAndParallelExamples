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
    }
}
