using System.Threading;
using UnityEngine;

public class ConnectToTRIK 
{
    private static Server recieveServer;
    private static Server sendServer;

    private static ObjectManager manager;

    private delegate void StopButtonHandler(bool paused);
    private static event StopButtonHandler NotifyRunStopButton;

    private delegate void RestartButtonHandler();
    private static event RestartButtonHandler NotifyRestartButton;

    private static bool paused = false;

    public static string ReceiveServerHost { get; private set; } = "127.0.0.1";
    public static string SendServerHost { get; private set; } = "127.0.0.1";
    public static int ReceiveServerPort { get; private set; } = 8080;
    public static int SendServerPort { get; private set; } = 9000;

    public static void Connect()
    {
        manager = GameObject.Find("ButtonScripts").GetComponent<ObjectManager>();

        recieveServer = new Server();
        sendServer = new Server();

        NotifyRunStopButton += sendServer.RunStopPressed;
        NotifyRestartButton += sendServer.RestartPressed;

        new Thread(() => recieveServer.StartServer(ReceiveServerHost, ReceiveServerPort, Server.ServerType.Recieve, manager)).Start();
        new Thread(() => sendServer.StartServer(SendServerHost, SendServerPort, Server.ServerType.Send, manager)).Start();
    }

    public static void StopServers()
    {
        recieveServer.StopServer();
        sendServer.StopServer();
    }

    public static void RunStopButtonPressed()
    {
        NotifyRunStopButton.Invoke(paused);
        paused = !paused;
    }

    public static void RestartPressed() => NotifyRestartButton.Invoke();
}
