using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vicold.Atmospex.Data.DataMemory
{
    internal class ShortArrayGridDataMemory : IGridDataMemory
    {
        private GCHandle _lockedData;
        private ArraySegment<short> _memory;

        public ShortArrayGridDataMemory(short[] buffer)
        {
            _memory = new ArraySegment<short>(buffer);
        }

        public ShortArrayGridDataMemory(short[] buffer, int offset, int length)
        {
            _memory = new ArraySegment<short>(buffer, offset, length);
        }

        public void Dispose()
        {
            if (_lockedData.IsAllocated)
                _lockedData.Free();
        }

        public IntPtr Lock()
        {
            if (!_lockedData.IsAllocated)
                _lockedData = GCHandle.Alloc(_memory.Array, GCHandleType.Pinned);
            return IntPtr.Add(_lockedData.AddrOfPinnedObject(), _memory.Offset * sizeof(short));
        }

        public unsafe byte ReadByte(int offset)
        {
            fixed (short* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *(pb + _memory.Offset * sizeof(short) + offset);
            }
        }

        public ushort ReadUInt16(int offset)
        {
            return Convert.ToUInt16(_memory.Array[_memory.Offset + offset / sizeof(short)]);
        }

        public short ReadInt16(int offset)
        {
            return _memory.Array[_memory.Offset + offset / sizeof(short)];
        }

        public unsafe uint ReadUInt32(int offset)
        {
            fixed (short* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((uint*)(pb + _memory.Offset * sizeof(short) + offset));
            }
        }

        public unsafe int ReadInt32(int offset)
        {
            fixed (short* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((int*)(pb + _memory.Offset * sizeof(short) + offset));
            }
        }

        public unsafe float ReadFloat(int offset)
        {
            fixed (short* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((float*)(pb + _memory.Offset * sizeof(short) + offset));
            }
        }

        public void Read(int offset, IntPtr dst, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(short) + _memory.Offset, dst, length / sizeof(short));
        }

        public void Unlock()
        {
            if (_lockedData.IsAllocated)
                _lockedData.Free();
        }

        public unsafe void UnsafeAction(Action<IntPtr> action)
        {
            fixed (short* ptr = _memory.Array)
            {
                action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public unsafe T UnsafeAction<T>(Func<IntPtr, T> action)
        {
            fixed (short* ptr = _memory.Array)
            {
                return action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public void Write(int offset, IntPtr src, int length)
        {
            Marshal.Copy(src, _memory.Array, offset / sizeof(short) + _memory.Offset, length / sizeof(short));
        }

        public void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(short) + _memory.Offset,
                IntPtr.Add(dst.Lock(), dstOffset), length / sizeof(short));
            dst.Unlock();
        }
    }
}