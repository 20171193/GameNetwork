using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 포인트
// 1. ThreadLocal
//  - 각 스레드 별로 Buffer를 가져 스레드 안전성 확보
//  - 최초 초기화, 재사용되는 풀링방식으로 효율적
//
//  but* 스레드 수가 증가하면 메모리 폭증 가능성
//      - 스레드 수가 제한적이며, 동기화를 피해야 하는 환경에서 사용.

namespace ServerCore
{
    public static class SendBufferHelper
    {
        // 컨텐츠 간 경합방지
        private static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(()=> { return null; });

        // UDP 최대 크기 : 65535
        public static int ChunkSize { get; set; } = 65535 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null || CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }
    
    /// <summary>
    /// 보낼 데이터를 구성하는 임시 공간
    /// </summary>
    internal class SendBuffer
    {
        // 버퍼배열
        private byte[] _buffer;

        // 버퍼배열의 커서
        private int _usedSize = 0;

        // 남은 공간
        public int FreeSize{ get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
