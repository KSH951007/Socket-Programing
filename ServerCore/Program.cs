using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welecom to MMORPG Server !");
            Send(sendBuff);

            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, 0, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");

        }
    }

    internal class Program
    {
        static Listener listener = new Listener();

      
        static void Main(string[] args)
        {
            //소캣생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);

           
            try
            {

                listener.Init(endPoint, () => { return new GameSession(); }) ;
                while (true)
                {
                   

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
          

        }

    }

}
