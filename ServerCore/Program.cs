using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{

    internal class Program
    {
        static Listener listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                byte[] recvBuff = new byte[1024];

                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                byte[] sendBuff = Encoding.UTF8.GetBytes("Welecom to MMORPG Server !");
                clientSocket.Send(sendBuff);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        static void Main(string[] args)
        {
            //소캣생성
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);

           
            try
            {

                listener.Init(endPoint, OnAcceptHandler) ;
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
