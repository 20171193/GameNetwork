using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        private Socket _listenSocket;

        /// <param name="register"> 비동기 요청 개수 </param>
        /// <param name="backlog"> 버퍼링 최대개수 </param>
        public void Init(IPEndPoint endPoint, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(endPoint);

            // 작업개시
            _listenSocket.Listen(backlog);

            for(int i =0; i<register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        public void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 재등록 오류 방지
            args.AcceptSocket = null;
            
            // 비동기 큐 할당
            bool pending = _listenSocket.AcceptAsync(args);

            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                // 세션 생성

                // 연결
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);
        }
    }
}
