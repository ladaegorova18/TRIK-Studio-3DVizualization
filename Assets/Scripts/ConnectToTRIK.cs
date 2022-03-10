using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectToTRIK 
{
    public static void Connect()
    {
        new Thread(() => Listener("127.0.0.1", 8888)).Start();
        //new Thread(() => Listener("127.0.0.1", 9000)).Start();
    }

    private static void Listener(string host, int port)
    {
        Debug.Log("Server started");
        var localAddr = IPAddress.Parse(host);
        var server1 = new TcpListener(localAddr, port);
        server1.Start();
        try
        {
            while (true)
            {
                var client = server1.AcceptTcpClient();
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
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        var message = builder.ToString();
                        Debug.Log("Сервер: " + message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
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
            server1.Stop();
        }
    }
}
