using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
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
        }
    }
}