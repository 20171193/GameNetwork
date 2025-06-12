using System;
using System.Xml;

namespace PacketGenerator
{
    // XML 패킷 정의
    class Program
    {
        static string genPackets;
        static ushort packetId;
        static string packetEnums;

        static string clientRegister;
        static string serverRegister;

        static void Main(string[] args)
        {
            // 실행위치 기준 상위 폴더 
            string pdlPath = "../PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,  // 주석 무시
                IgnoreWhitespace = true // 불필요한 공백 노드 무시
            };

            if (args.Length >= 1)
                pdlPath = args[0];

            using (XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();

                // 컨텐츠 모든 노드 순회
                while(r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                }


            }
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine($"ParsePacket Error : Invalid packet node (Node Name : {r.Name})");
                return;
            }

            string packetName = r["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);

        }
        
        public static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            string memberCode = ""; // {1} : 멤버 변수
            string readCode = "";   // {2} : 멤버 변수 Read
            string writeCode = "";  // {3} : 멤버 변수 Write

            int depth = r.Depth + 1;
            while(r.Read())
            {
                // 직속 노드만 파싱
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                { 
                    Console.WriteLine("Member without name");
                    return null;
                }

                // 줄바꿈 추가
                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();

                switch (memberType)
                {
                    case "byte":
                        // sbyte 사용 시 Packet Generator에서 별도의 타입 캐스팅이 필요.
                        // case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;
                }
            }

            // 정렬
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];
            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> t = ParseMembers(r);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }
        // {1} To~ 변수 형식
        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }
        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }

    }
}
