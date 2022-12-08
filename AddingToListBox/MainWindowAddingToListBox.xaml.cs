using System.Windows;
using System.ComponentModel;
using System.Threading;

namespace BackgroundWorkerExamples
{
    /// <summary>
    /// Interaction logic for MainWindowAddingToListBox.xaml
    /// </summary>
    public partial class MainWindowAddingToListBox : Window
    {
        private const int ITERATIONS_COUNT = 100;
        private readonly BackgroundWorker addingToListBoxWorker = new();

        public MainWindowAddingToListBox()
        {
            InitializeComponent();
            startButton.Focus();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            addingToListBoxWorker.WorkerReportsProgress = true;
            addingToListBoxWorker.WorkerSupportsCancellation = true;

            addingToListBoxWorker.DoWork += AddingToListBoxWorker_DoWork;
            addingToListBoxWorker.ProgressChanged += AddingToListBoxWorker_ProgressChanged;
            addingToListBoxWorker.RunWorkerCompleted += AddingToListBoxWorker_RunWorkerCompleted;
        }

        private void AddingToListBoxWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            // Do not access the window's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            if (sender is not BackgroundWorker bgWorker)
            {
                return;
            }

            // Extract the argument.
            int iterCount = e.Argument is null ? ITERATIONS_COUNT : (int)e.Argument;

            // If there is a result of the computation, then you can assign the result
            // to the DoWorkEventArgs.Result property.
            // This is will be available to the RunWorkerCompleted event handler.
            ConsumeTimeInOperations(bgWorker, iterCount);

            // If the operation was canceled by the user,
            // set the DoWorkEventArgs.Cancel property to true.
            // This verification here is very important because:
            // Note that a call to CancelAsync may have set
            // CancellationPending to true just after the
            // last verification to bgWorker.CancellationPending
            // in ConsumeTimeInOperations method in the last iteration,
            // so the code in ConsumeTimeInOperations method
            // will not have the opportunity to set
            // the DoWorkEventArgs.Cancel flag to true. This means
            // that RunWorkerCompletedEventArgs.Cancelled will
            // not be set to true in your RunWorkerCompleted
            // event handler. This is a race condition.
            if (bgWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private static void ConsumeTimeInOperations(BackgroundWorker bgWorker, int iterCount)
        {
            Thread.Sleep(1000);

            for (var i = 0; i <= iterCount; ++i)
            {
                if (bgWorker.CancellationPending)
                {
                    Thread.Sleep(1000);

                    break;
                }

                bgWorker.ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        private void AddingToListBoxWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            var progressPercentage = e.ProgressPercentage;

            listBox.Items.Add($"{progressPercentage} item added");
            addingProgressBar.Value = progressPercentage;
            statusLabel.Content = $"Running, {progressPercentage} % completed";
        }

        private void AddingToListBoxWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error is not null)
            {
                statusLabel.Content = "Error: " + e.Error.Message;
            }
            else if (e.Cancelled)
            {
                statusLabel.Content = "Cancelled";
            }
            else
            {
                statusLabel.Content = "Completed";
            }

            startButton.IsEnabled = true;
            startButton.Focus();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            addingProgressBar.Value = 0;
            statusLabel.Content = "Running...";
            startButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
            cancelButton.Focus();

            addingToListBoxWorker.RunWorkerAsync(ITERATIONS_COUNT);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            statusLabel.Content = "Cancelling...";
            cancelButton.IsEnabled = false;

            addingToListBoxWorker.CancelAsync();
        }
    }
}
