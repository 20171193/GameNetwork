using System.Net;
using System.Net.Sockets;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250); // 반복
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            // 호스트의 이름으로 로컬 IP 
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            // AddressList : 유선/무선, VPN, 가상 네트워크 어댑터가 모두 포함된 배열
            // AddressFamily
            //  - InterNetwork : IPv4 
            //  - InterNetworkV6 : IPv6 
            IPAddress ipAddr
                = ipHost.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);

            // IP + 포트, 7777(시스템 예약 포트를 피한 테스트 포트)
            // 0~1023 : (시스템 예약 포트)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            // 최초 Flush
            JobTimer.Instance.Push(FlushRoom);

            // 메인 루프
            while(true)
            {
                JobTimer.Instance.Flush();
                // 1. CPU 점유를 줄일 수 있는 방법?
                // 2. 종료 루틴의 필요성?
                //  2-1. 종료 시 상태 저장
                //  2-2. 종료 시 세션 정리
            }
        }
    }
}