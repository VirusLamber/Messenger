using System;
using System.Windows;
using System.Windows.Threading;

namespace Server
{
    public partial class MainWindow : Window
    {
        private ChatServer _server;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Timer for future features
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = txtIp.Text;
                int port = int.Parse(txtPort.Text);

                _server = new ChatServer(LogMessage);
                _server.Start(ip, port);

                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                statusText.Text = "Server running";
                LogMessage("Server started successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _server?.Stop();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            statusText.Text = "Server stopped";
            LogMessage("Server stopped");
        }

        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                txtLog.ScrollToEnd();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _server?.Stop();
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}