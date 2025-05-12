using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ChatServer
    {
        const int port = 4040;
        const string ip = "127.0.0.1";
        TcpListener server;
        List<TcpClient> clients;
        Semaphore semaphore = new Semaphore(10, 10);

        public ChatServer()
        {
            server = new TcpListener(System.Net.IPAddress.Parse(ip), port);
            server.Start();
            clients = new List<TcpClient>();
        }

        public void StartClient()
        {
            Console.WriteLine("Server started!");
            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    Task.Run(() => Start(client));
                    clients.Add(client);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void Start(TcpClient client)
        {
            if(!semaphore.WaitOne(200))
            {
                using (StreamWriter sww= new StreamWriter(client.GetStream()))
                {
                    sww.WriteLine("Server Error!");
                }
                client.Close();
                return;
            }
            NetworkStream ns = client.GetStream();
            Console.WriteLine("Connected!");
            StreamReader sr = new(ns);
            StreamWriter sw = new(ns);
            sw.AutoFlush = true;
            while(true)
            {
                try
                {
                    string message = sr.ReadLine();
                    if (message == null) break;

                Console.WriteLine($"Got message from  {client.Client.LocalEndPoint} {DateTime.Now} :: {message}");
                    foreach (var clienT in clients)
                    {
                        try
                        {
                            using var clientStream = clienT.GetStream();
                            using var clientWriter = new StreamWriter(clientStream) { AutoFlush = true };
                            clientWriter.WriteLine(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending message to client: {ex.Message}");
                        }
                    }

                }

                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                finally
                {
                    semaphore.Release();
                    client.Close();
                    Console.WriteLine("Client disconnected!");
                }

            }
            
            
        }
    }
}
