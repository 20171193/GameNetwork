using System;
using System.Net;
using DummyClient.Session;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DSN : 이름으로 IP 주소 찾기
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                5);
                
            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // 1초에 4번 정도
                Thread.Sleep(250);
            }
        }
    }
}