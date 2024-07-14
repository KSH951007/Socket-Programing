using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{


    internal abstract class Session
    {
        Socket socket;
        int disconnected = 0;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        Queue<byte[]> sendQueue = new Queue<byte[]>();
        object lockObj = new object();

        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);


        public abstract void OnDisconnected(EndPoint endPoint);
        public void Start(Socket socket)
        {
            this.socket = socket;
            
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }
        public void Send(byte[] sendBuff)
        {

            lock (lockObj)
            {
                sendQueue.Enqueue(sendBuff);
                if (pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }

        }
        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1)
            {
                return;
            }
            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

        }

        #region 네트워크 통신

        public void RegisterSend()
        {


            while (sendQueue.Count > 0)
            {
                byte[] buff = sendQueue.Dequeue();

                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            sendArgs.BufferList = pendingList;

            bool pending = socket.SendAsync(sendArgs);
            if (pending == false)
                OnSendCompleted(null, sendArgs);

        }
        public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {

            lock (lockObj)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e.Message}");
                    }
                }
                else
                    Disconnect();

            }
        }
        public void RegisterRecv()
        {
            bool pending = socket.ReceiveAsync(recvArgs);
            if (pending == false)
                OnRecvCompleted(null, recvArgs);


        }
        public void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e.Message}");
                }
            }
            else
                Disconnect();



        }
    }
    #endregion
}