using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// 포인트
// 1. read, write 두 개의 포인터를 사용하는 이유
//  - read와 write의 순서는 보장되지 않음.
//  - 빈 공간 shift를 위해 (압축)
//
// 2. 너무 큰 패킷
//  - RecvBuffer 풀링
//  - 다단계 버퍼 풀

namespace ServerCore
{
    public class RecvBuffer
    {
        private ArraySegment<byte> _buffer;
        private int _readPos;
        private int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        // 데이터 공간
        public int DataSize { get { return _writePos - _readPos; } }
        // 여유 공간
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;

            if (dataSize == 0)
                _readPos = _writePos = 0;
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);

                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
