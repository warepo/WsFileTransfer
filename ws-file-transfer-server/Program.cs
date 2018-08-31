using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ws_file_transfer_server
{
    class Program
    {
        static int fileCounter = 0;

        static void Main(string[] args)
        {
            StartListening();

            Console.WriteLine("Pressione ENTER para finalizar.");
            Console.Read();
        }

        static void StartListening()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("<<< Async TPC Listener >>>");
                Console.WriteLine();

                IPEndPoint server = null;

                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        server = new IPEndPoint(ip, 11000);
                    }
                }

                var listener = new TcpListener(server);

                listener.Start();

                Console.WriteLine($"Servidor {listener.LocalEndpoint}...");

                WaitForClients(listener);

                Console.WriteLine("Aguardando transferência de arquivos...");
            }
            catch (System.Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                throw;
            }

            return;
        }

        private static void WaitForClients(TcpListener listener)
        {
            try
            {
                listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), listener);
            }
            catch (System.Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                var listener = ar.AsyncState as TcpListener;
                var handler = listener.EndAcceptTcpClient(ar);

                Console.WriteLine();
                Console.WriteLine($"Connectado ao cliente {handler.Client.RemoteEndPoint}...");

                WaitForClients(listener);

                Console.WriteLine("Recebendo arquivo...");

                var networkStream = new NetworkStream(handler.Client);

                int bufferSize = 1024;

                var buffer = new byte[bufferSize];

                string fileName = $"ReceivedFile-{fileCounter}.txt";

                fileCounter++;

                var fileStream = File.OpenWrite(fileName);

                int bytesRead = -1;

                while (bytesRead != 0)
                {
                    bytesRead = networkStream.Read(buffer, 0, bufferSize);
                    fileStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();
                networkStream.Close();

                Console.WriteLine("Arquivo recebido.");
            }
            catch (System.Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
