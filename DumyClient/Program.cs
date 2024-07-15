using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DumyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
         
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World {i}");
                Send(sendBuff);
            }

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, 0, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");

        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TestClient";

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);
            Connector connector = new Connector();

           

            while (true)
            {

                try
                {
                    connector.Connect(endPoint, () => { return new GameSession(); });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(100);
            }


        }
    }
}
