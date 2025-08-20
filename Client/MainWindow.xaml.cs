using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Client
{
    public partial class MainWindow : Window
    {
        private ChatClient _client;
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

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = txtServerIp.Text;
                int port = int.Parse(txtServerPort.Text);
                string username = txtUsername.Text;

                _client = new ChatClient(AddMessage);
                if (_client.Connect(ip, port, username))
                {
                    btnConnect.IsEnabled = false;
                    btnDisconnect.IsEnabled = true;
                    btnSend.IsEnabled = true;
                    txtMessage.IsEnabled = true;
                    AddMessage($"Connected to {ip}:{port}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _client?.Disconnect();
            btnConnect.IsEnabled = true;
            btnDisconnect.IsEnabled = false;
            btnSend.IsEnabled = false;
            txtMessage.IsEnabled = false;
            AddMessage("Disconnected from server");
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(message) && _client != null && _client.IsConnected)
            {
                string fullMessage = $"{txtUsername.Text}: {message}";
                _client.SendMessage(fullMessage);
                AddMessage($"You: {message}");
                txtMessage.Clear();
            }
        }

        private void AddMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtChat.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                txtChat.ScrollToEnd();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _client?.Disconnect();
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}