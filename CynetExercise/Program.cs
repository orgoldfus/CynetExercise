using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CynetExercise
{
    class ClientContext
    {
        public TcpClient Client;
        public NetworkStream Stream;
        public byte[] Buffer = new byte[4096];
    }

    class Program
    {
        private static List<Task> _logTasks = new List<Task>();
        static void Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 7777);
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();

            listener.BeginAcceptTcpClient(OnClientAccepted, listener);

            Console.Write("Press enter to exit...");
            Console.ReadLine();
            Task.WaitAll(_logTasks.ToArray());
            listener.Stop();
        }

        static void OnClientAccepted(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;

            if (listener == null)
                return;

            try
            {
                ClientContext context = new ClientContext();
                context.Client = listener.EndAcceptTcpClient(ar);
                context.Stream = context.Client.GetStream();
                context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, OnClientRead, context);
            }
            finally
            {
                listener.BeginAcceptTcpClient(OnClientAccepted, listener);
            }
        }

        static void OnClientRead(IAsyncResult ar)
        {
            ClientContext context = ar.AsyncState as ClientContext;

            try
            {
                int bytesRead = context.Stream.EndRead(ar);
                string message = Encoding.ASCII.GetString(context.Buffer, 0, bytesRead);
                _logTasks.Add(Logger.LogAsync(message));
            }
            finally
            {
                context.Stream.Close();
                context.Client.Close();
                context = null;
            }
        }
    }
}
