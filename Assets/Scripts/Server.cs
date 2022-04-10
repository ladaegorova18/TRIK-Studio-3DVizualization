using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Server
{
    private TcpListener listener;
    private NetworkStream stream;
    ObjectManager manager;
    TcpClient client;

    public enum ServerType
    {
        Recieve,
        Send
    }

    public void StartServer(string host, int port, ServerType type, ObjectManager _manager)
    {
        Debug.Log("Server started");
        manager = _manager;
        var localAddr = IPAddress.Parse(host);
        listener = new TcpListener(localAddr, port);
        listener.Start();

        try
        {
            while (true)
            {
                client = listener.AcceptTcpClient();
                Debug.Log(client.Client.LocalEndPoint);
                try
                {
                    stream = client.GetStream();

                    if (type == ServerType.Recieve)
                    {
                        ReceiveMessage();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void RunStopPressed(bool paused)
    {
        string message = paused ? "Run" : "Stop";
        SendMessage(message);
    }

    public void RestartPressed()
    {
        SendMessage("Restart");
    }

    /// <summary>
    /// Receives message from client
    /// </summary>
    private void ReceiveMessage()
    {
        while (true)
        {
            var data = new byte[64];
            var builder = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            manager.AddFrame(builder.ToString());
            Debug.Log(builder.ToString());
        }
    }

    /// <summary>
    /// Sends message in UTF8 to client (pause, play, restart)
    /// </summary>
    private void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
            if (stream.CanWrite)
            {
                stream.Write(data, 0, data.Length);
                Debug.Log($"{message} sent");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// Stops listener and client
    /// </summary>
    public void StopServer()
    {
        client.Close();
        listener?.Stop();
        Debug.Log("Server stopped");
    }
}
