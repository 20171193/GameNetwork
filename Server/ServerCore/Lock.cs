using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    // 재귀적 락 허용 O
    //  : WrtierLock -> WrtierLock  : O
    //  : WrtierLock -> ReadLock    : O
    //  : ReadLock -> WriterLock    : X

    // 스핀락 5,000
    
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;

        // 0(mst)111 1111 1111 1111
        // 0000 0000 0000 0000
        const int WRITE_MASK = 0x7FFF0000;

        // 0000 0000 0000 0000
        // 1111 1111 1111 1111
        const int READ_MASK = 0x0000FFFF;

        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1)] [WriteThreadID(15)] [ReadCount(16)]
        int _flag;

        int _writeCount = 0;

        #region Write (독점)
        public void WriteLock()
        {
            // 재귀적 락 허용
            //  : WriteLock -> WriteLock (O)
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                // 카운팅만 진행
                _writeCount++;
                return;
            }

            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while(true)
            {
                for(int i =0; i<MAX_SPIN_COUNT; i++)
                {
                    // EMPTY_FLAG인 경우 소유권 획득
                    if(Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                // 스핀후 양보
                Thread.Yield();
            }

        }
        public void WriteUnLock()
        {
            // 재귀적 락 허용
            //  : Write Count에 따라 UnLock
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }
        #endregion

        #region Read (다중 획득)
        public void ReadLock()
        {
            // 재귀적 락 허용
            //  : WriteLock -> ReadLock (X)
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            while(true)
            {
                for(int i =0; i<MAX_SPIN_COUNT; i++)
                {
                    // WriteLock을 획득하고 있지 않은 경우 ReadCount증가
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
                Thread.Yield();
            }
        }
        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }
        #endregion
    }
}
