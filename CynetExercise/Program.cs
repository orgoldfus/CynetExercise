using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        static void OnClientRead(IAsyncResult ar)
        {
            ClientContext context = ar.AsyncState as ClientContext;
            if (context == null)
                return;

            try
            {
                int bytesRead = context.Stream.EndRead(ar);
                string message = Encoding.ASCII.GetString(context.Buffer, 0, bytesRead);
                Logger.log(message);
            }
            finally
            {
                context.Stream.Close();
                context.Client.Close();
                context.Buffer = null;
                context = null;
            }
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

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 7777));
            listener.Start();

            listener.BeginAcceptTcpClient(OnClientAccepted, listener);

            Console.Write("Press enter to exit...");
            Console.ReadLine();
            listener.Stop();
        }
    }
}
