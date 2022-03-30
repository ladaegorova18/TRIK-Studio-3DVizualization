using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectToTRIK 
{
    private static TcpListener listener;

    static ObjectManager manager;

    void Awake()
    {
    }

    public static void Connect()
    {
        manager = GameObject.Find("ButtonScripts").GetComponent<ObjectManager>();
        new Thread(() => Listener("127.0.0.1", 8080)).Start();
    }

    private static void Listener(string host, int port)
    {
        Debug.Log("Server started");
        var localAddr = IPAddress.Parse(host);
        listener = new TcpListener(localAddr, port);
        listener.Start();
        Debug.Log("Server started");

        try
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Debug.Log(client.Client.LocalEndPoint);
                try
                {
                    var stream = client.GetStream();

                    while (true)
                    {
                        //Console.Write(userName + ": ");
                        //// ввод сообщения
                        //string message = Console.ReadLine();
                        //message = String.Format("{0}: {1}", userName, message);
                        //// преобразуем сообщение в массив байтов
                        //byte[] data = Encoding.Unicode.GetBytes(message);
                        //// отправка сообщения
                        //stream.Write(data, 0, data.Length);

                        // получаем ответ
                        var data = new byte[64]; // буфер для получаемых данных
                        var builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            //builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        var frameString = builder.ToString();
                        //if (frameString != "" && frameString != null)
                        //{
                        //    Debug.Log("Сервер: " + frameString);
                        //}
                        manager.AddFrame(frameString);
                        //manager.PlayFrameFrom();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Debug.Log(e);
                }
                finally
                {
                    client.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            listener.Stop();
        }
        }

    public static void StopServer()
    {
        listener.Stop();
        Debug.Log("Server stopped");
    }
}
