using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public class ChatClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _serverIp;
        private int _serverPort;
        private bool _isConnected;
        private Action<string> _messageReceivedAction;

        public ChatClient(Action<string> messageReceivedAction)
        {
            _messageReceivedAction = messageReceivedAction;
        }

        public bool Connect(string serverIp, int serverPort, string username)
        {
            try
            {
                _serverIp = serverIp;
                _serverPort = serverPort;

                _client = new TcpClient();
                _client.Connect(serverIp, serverPort);
                _stream = _client.GetStream();
                _isConnected = true;

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                SendMessage($"{username} joined the chat");
                return true;
            }
            catch (Exception ex)
            {
                _messageReceivedAction?.Invoke($"Connection error: {ex.Message}");
                return false;
            }
        }

        public void SendMessage(string message)
        {
            if (_isConnected && _stream != null)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    _stream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    _messageReceivedAction?.Invoke($"Send error: {ex.Message}");
                    Disconnect();
                }
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (_isConnected)
            {
                try
                {
                    bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Disconnect();
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    _messageReceivedAction?.Invoke(message);
                }
                catch
                {
                    Disconnect();
                    break;
                }
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _isConnected = false;
                _stream?.Close();
                _client?.Close();
                _messageReceivedAction?.Invoke("Disconnected from server");
            }
        }

        public bool IsConnected => _isConnected;
    }
}