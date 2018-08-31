using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ws_file_transfer_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<<< Async TCP Client >>>");

            Console.Write("IP do servidor: ");

            var ipAddress = IPAddress.Parse(Console.ReadLine());

            var server = new IPEndPoint(ipAddress, 11000);

            var client = new TcpClient();

            client.Connect(server);

            Console.WriteLine($"Conectado a {client.Client.RemoteEndPoint}.");

            Console.WriteLine("Preparando arquivo para envio...");

            var fileStream = File.OpenRead("FileToSend.txt");

            var fileBuffer = new byte[fileStream.Length];

            Console.WriteLine("Lendo arquivo...");

            fileStream.Read(fileBuffer, 0, (int)fileStream.Length);

            var networkStream = client.GetStream();

            Console.WriteLine("Enviando arquivo...");

            networkStream.Write(fileBuffer, 0, (int)fileStream.Length);

            networkStream.Close();
            fileStream.Close();

            Console.WriteLine("Arquivo enviado.");

            client.Close();

            Console.WriteLine("Conexão encerrada.");
        }
    }
}
