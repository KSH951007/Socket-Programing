using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {

        Socket listenSocket;
        Func<Session> sessionFactory;
        const int LISTEN_COUNT = 10;
        /*
        
        TCP 소켓 프로그래밍 흐름

         클라이언트 소켓의 흐름
        1. 클라이언트 소켓을 생성
        2. 서버소켓에 연결을 요청
        3. 연결이 받아들여지면 데이터를 송/수신
        4. 모든 처리가 완료되면 소켓을 Close


         서버 소켓의 흐름
        1. 서버 소켓을 생성
        2. 서버가 사용할 IP주소와 포트번호를 생성한 소켓에 결합(Bind)
        3. 클리이언트로부터 연결 요청이 수신되는지 주시(Listen)
        4. 요청이 수신되면 받아드려(Accept) 데이터 통신을 위한 소켓을 생성
        5. 새로운 소켓을 통해 연결이 수립되면 클라이언트와 마찬가지로 데이터를 송/수신
        6. 데이터가 송/수신이 완료되면 소켓을 Close


        Send/Recsive 는 블럭(Block)방식으로 동작한다.
        두 함수는 실행 결과가 결정되기전까지 리턴하지않는다
        Send의 경우 데이터를 보내는 주체가 자기 자신이기 때문에 얼마만큼 데이터를 보낼 것인지,언제 보낼 것인지를 알수있다
        하지만Recsive는  대상이 언제,어떤 데이터를 보낼것인지 특정할 수없기떄문에 Recsive는 한번 실행되면 언제 끝날지 모르는 상태이다.
        
        그래서 데이터를 수신 하기 위한 Recsive는 별도의 스레드에서 실행된다(멀티 쓰레딩) 소켓이 생성과 연결이 완료 된 후, 
        새로운 스레드를 하나 만든다음 그곳에서 Recsive를 실행하고 데이터 수신을 되기까지 대기

         */
        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory += sessionFactory;
            listenSocket.Bind(endPoint);
            
            listenSocket.Listen(LISTEN_COUNT);
            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(eventArgs);
        }

        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool isPending = listenSocket.AcceptAsync(args);
            if (isPending == false)
            {
                OnAcceptCompleted(null, args);
            }


        }
        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {

                Session session = sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }
    }
}
