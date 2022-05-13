using System.Threading;

/// <summary>
/// Handles a connection to TRIK Studio using two servers: to send and to receive data
/// </summary>
public class ConnectionManager 
{
	/// <summary>
	/// Receives trajectory
	/// </summary>
	private static Server recieveServer;

	/// <summary>
	/// Sends button signals: run/stop, restart
	/// </summary>
	private static Server sendServer;

	private delegate void PauseButtonHandler(bool paused);
	private static event PauseButtonHandler NotifyRunPauseButton;

	private delegate void RestartButtonHandler();
	private static event RestartButtonHandler NotifyRestartButton;

	private static bool paused = false;

	public static string ReceiveServerHost { get; private set; } = "127.0.0.1";
	public static string SendServerHost { get; private set; } = "127.0.0.1";
	public static int ReceiveServerPort { get; private set; } = 8080;
	public static int SendServerPort { get; private set; } = 9000;

	public static void Connect()
	{
		recieveServer = new Server();
		sendServer = new Server();

		NotifyRunPauseButton += sendServer.RunPausePressed;
		NotifyRestartButton += sendServer.RestartPressed;

		new Thread(() => recieveServer.StartServer(ReceiveServerHost, ReceiveServerPort)).Start();
		new Thread(() => sendServer.StartServer(SendServerHost, SendServerPort)).Start();
	}

	/// <summary>
	/// Stops both servers and closes the connection
	/// </summary>
	public static void StopServers()
	{
		recieveServer?.StopServer();
		sendServer?.StopServer();
	}

	public static void RunPausePressed()
	{
		NotifyRunPauseButton.Invoke(paused);
		paused = !paused;
	}

	public static void RestartPressed() => NotifyRestartButton.Invoke();
}
