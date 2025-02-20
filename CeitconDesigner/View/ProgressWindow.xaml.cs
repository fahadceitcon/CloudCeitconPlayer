using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Ceitcon_Designer.View
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    /// 
    public partial class ProgressWindow : IProgressable
    {
        public event Action CancelCallback;
        private TaskbarItemInfo _taskbarItemInfo;

        public ProgressWindow()
        {
            InitializeComponent();
        }
      

        public void Attach(TaskbarItemInfo taskbarItemInfo)
        {
            _taskbarItemInfo = taskbarItemInfo;
            if (taskbarItemInfo != null)
            {
                progressBar.ValueChanged += OnValueChanged;
            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _taskbarItemInfo.ProgressValue = e.NewValue / 100;
        }

        public void SetMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
                {
                    messageText.Text = message;
                });
            }
            else
            {
                messageText.Text = message;
            }
        }

        public void SetIndeterminate(bool isIndeterminate)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
                {
                    progressBar.IsIndeterminate = isIndeterminate;
                });
            }
            else
            {
                progressBar.IsIndeterminate = isIndeterminate;
            }
        }

        public void SetMessage(string message, bool isIndeterminate)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
                {
                    messageText.Text = message;
                    progressBar.IsIndeterminate = isIndeterminate;
                    if (isIndeterminate)
                        progressBar.Value = 0;
                });
            }
            else
            {
                messageText.Text = message;
                progressBar.IsIndeterminate = isIndeterminate;
                if (isIndeterminate)
                    progressBar.Value = 0;
            }
        }

        public void SetProgress(double value, string message)
        {
            progressBar.IsIndeterminate = false;
            progressBar.Value = GetSafeProgressValue(value);

            if (!string.IsNullOrEmpty(message))
            {
                messageText.Text = message;
            }
        }

        public void SafeSetProgress(double value, string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
                {
                    progressBar.IsIndeterminate = value == -1;
                    progressBar.Value = GetSafeProgressValue(value);

                    if (!string.IsNullOrEmpty(message))
                    {
                        messageText.Text = message;
                    }
                });
            }

            else SetProgress(value, message);
        }

        public void SafeShow(string message)
        {
            messageText.Text = message;
            cancelButton.Visibility = Visibility.Collapsed;

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)Show);
            }

            else Show();
        }

        public void SafeShow()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)Show);
            }

            else Show();
        }

        public void SafeHide()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)Hide);
            }

            else Hide();
        }

        public void Invoke(Action action)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }

            else action();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            messageText.Text = "Looks up a localized string similar to Canceling. Please wait....";

            IsCancelled = true;

            if (CancelCallback != null)
                CancelCallback();
        }

        private double GetSafeProgressValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 0;

            return value;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        #region IProgressable Members

        public void OnProgress(double progress, string message)
        {
            SafeSetProgress(progress, message);
        }

        public bool IsCancelled { get; set; }
        #endregion

        public void Dispose()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)Close);
            }

            else Close();
        }
    }

    public interface IProgressable : IDisposable
    {
        bool IsCancelled { get; }
        void SetIndeterminate(bool isIndeterminate);
        void SetMessage(string message, bool isIndeterminate);
        void OnProgress(double progress, string message);
        void Hide();
        void Show();
        void Invoke(Action action);
    }

    public class ProgressInfo
    {
        public double Progress { get; set; }
        public string Message { get; set; }
    }
}