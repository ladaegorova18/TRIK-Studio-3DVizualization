using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Server
{
	private TcpListener listener;
	private NetworkStream stream;
	private bool isOn = true;
	TcpClient client;

	public enum ServerType { Recieve, Send }

	public void StartServer(string host, int port, ServerType type)
	{
		isOn = true;
		listener = new TcpListener(IPAddress.Parse(host), port);
		listener.Start();
		Debug.Log("Listening...");

		try
		{
			while (isOn)
			{
				client = listener.AcceptTcpClient();
				Debug.Log("Connected to: " + client.Client.LocalEndPoint);
				
				stream = client.GetStream();

				if (type == ServerType.Recieve)
					ReceiveMessage();
			}
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}

	public void RunPausePressed(bool paused)
	{
		var message = paused ? "Run" : "Stop";
		SendMessage(message);
	}

	public void RestartPressed() => SendMessage("Restart");

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
	/// Receives message from client
	/// </summary>
	private void ReceiveMessage()
	{
		while (isOn)
		{
			try
			{
				var data = new byte[64];
				var builder = new StringBuilder();

				do
				{
					var bytes = stream?.Read(data, 0, data.Length);
					if (bytes.HasValue)
						builder.Append(Encoding.UTF8.GetString(data, 0, bytes.Value));
				}
				while (stream.DataAvailable);

				Debug.Log(builder.ToString());
				Deserializer.ParseFrameFromString(builder.ToString());
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
		}
	}

	/// <summary>
	/// Stops listener and client
	/// </summary>
	public void StopServer()
	{
		client?.Close();
		listener?.Stop();
		isOn = false;
		Debug.Log("Server stopped");
	}
}
