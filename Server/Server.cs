using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
    public class ChatServer
    {
        private TcpListener _server;
        private List<TcpClient> _clients = new List<TcpClient>();
        private bool _isRunning;
        private Action<string> _logAction;

        public ChatServer(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Start(string ipAddress, int port)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                _server = new TcpListener(ip, port);
                _isRunning = true;
                _server.Start();

                Log($"Server started on {ipAddress}:{port}");

                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                Log($"Server start error: {ex.Message}");
            }
        }

        private void AcceptClients()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = _server.AcceptTcpClient();
                    _clients.Add(client);

                    Log($"New connection: {client.Client.RemoteEndPoint}");

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                        Log($"Accept client error: {ex.Message}");
                }
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while (_isRunning && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Log($"Received: {message}");

                    BroadcastMessage(message, client);
                }
            }
            catch (Exception ex)
            {
                Log($"Client error: {ex.Message}");
            }
            finally
            {
                _clients.Remove(client);
                client.Close();
                Log("Client disconnected");
            }
        }

        private void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var client in _clients.ToArray())
            {
                if (client != sender && client.Connected)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        Log($"Send error: {ex.Message}");
                    }
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            foreach (var client in _clients.ToArray())
            {
                client.Close();
            }
            _server?.Stop();
            Log("Server stopped");
        }

        private void Log(string message)
        {
            _logAction?.Invoke(message);
        }
    }
}