using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vicold.Atmospex.Data.DataMemory
{
    internal class UnsafeGridDataMemory : IGridDataMemory
    {
        private readonly Action<IntPtr> _disposeMethod;
        private readonly IntPtr _memory;

        public UnsafeGridDataMemory(IntPtr ptr, Action<IntPtr> dispose)
        {
            _memory = ptr;
            _disposeMethod = dispose;
        }

        public unsafe void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length)
        {
            Buffer.MemoryCopy(_memory.ToPointer(), IntPtr.Add(dst.Lock(), dstOffset).ToPointer(), length, length);
        }

        public void Dispose()
        {
            _disposeMethod(_memory);
        }

        public IntPtr Lock()
        {
            return _memory;
        }

        public unsafe byte ReadByte(int offset)
        {
            return Marshal.ReadByte(IntPtr.Add(_memory, offset));
        }

        public ushort ReadUInt16(int offset)
        {
            return Convert.ToUInt16(Marshal.ReadInt16(IntPtr.Add(_memory, offset)));
        }

        public short ReadInt16(int offset)
        {
            return Marshal.ReadInt16(IntPtr.Add(_memory, offset));
        }

        public uint ReadUInt32(int offset)
        {
            return Convert.ToUInt32(Marshal.ReadInt32(IntPtr.Add(_memory, offset)));
        }

        public int ReadInt32(int offset)
        {
            return Marshal.ReadInt32(IntPtr.Add(_memory, offset));
        }

        public float ReadFloat(int offset)
        {
            return Convert.ToSingle(Marshal.ReadInt32(IntPtr.Add(_memory, offset)));
        }

        public unsafe void Read(int offset, IntPtr dst, int length)
        {
            Buffer.MemoryCopy(IntPtr.Add(_memory, offset).ToPointer(), dst.ToPointer(), length, length);
        }

        public void Unlock()
        {
        }

        public void UnsafeAction(Action<IntPtr> action)
        {
            action(_memory);
        }

        public unsafe T UnsafeAction<T>(Func<IntPtr, T> action)
        {
            return action(_memory);
        }

        public unsafe void Write(int offset, IntPtr src, int length)
        {
            Buffer.MemoryCopy(src.ToPointer(), IntPtr.Add(_memory, offset).ToPointer(), length, length);
        }
    }
}