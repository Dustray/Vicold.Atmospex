using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.DataService.Provider
{
    public interface IGridDataMemory : IDisposable
    {
        IntPtr Lock();

        void Unlock();

        unsafe void UnsafeAction(Action<IntPtr> action);

        unsafe T UnsafeAction<T>(Func<IntPtr, T> action);

        byte ReadByte(int offset);

        ushort ReadUInt16(int offset);

        short ReadInt16(int offset);

        uint ReadUInt32(int offset);

        int ReadInt32(int offset);

        float ReadFloat(int offset);

        void Read(int offset, IntPtr dst, int length);

        void Write(int offset, IntPtr src, int length);

        void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length);
    }
}
