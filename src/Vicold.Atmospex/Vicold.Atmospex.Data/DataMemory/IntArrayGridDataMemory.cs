using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vicold.Atmospex.Data.DataMemory
{
    internal class IntArrayGridDataMemory : IGridDataMemory
    {
        private GCHandle _lockedData;
        private ArraySegment<int> _memory;

        public IntArrayGridDataMemory(int[] buffer)
        {
            _memory = new ArraySegment<int>(buffer);
        }

        public IntArrayGridDataMemory(int[] buffer, int offset, int length)
        {
            _memory = new ArraySegment<int>(buffer, offset, length);
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
            return IntPtr.Add(_lockedData.AddrOfPinnedObject(), _memory.Offset * sizeof(int));
        }

        public unsafe byte ReadByte(int offset)
        {
            fixed (int* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *(pb + _memory.Offset * sizeof(int) + offset);
            }
        }

        public unsafe ushort ReadUInt16(int offset)
        {
            fixed (int* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((ushort*)(pb + _memory.Offset * sizeof(float) + offset));
            }
        }

        public unsafe short ReadInt16(int offset)
        {
            fixed (int* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((short*)(pb + _memory.Offset * sizeof(float) + offset));
            }
        }

        public uint ReadUInt32(int offset)
        {
            return (uint)(_memory.Array[_memory.Offset + offset / sizeof(int)]);
        }

        public int ReadInt32(int offset)
        {
            return _memory.Array[_memory.Offset + offset / sizeof(int)];
        }

        public float ReadFloat(int offset)
        {
            return _memory.Array[_memory.Offset + offset / sizeof(int)];
        }

        public void Read(int offset, IntPtr dst, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(int) + _memory.Offset, dst, length / sizeof(int));
        }

        public void Unlock()
        {
            if (_lockedData.IsAllocated)
                _lockedData.Free();
        }

        public unsafe void UnsafeAction(Action<IntPtr> action)
        {
            fixed (int* ptr = _memory.Array)
            {
                action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public unsafe T UnsafeAction<T>(Func<IntPtr, T> action)
        {
            fixed (int* ptr = _memory.Array)
            {
                return action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public void Write(int offset, IntPtr src, int length)
        {
            Marshal.Copy(src, _memory.Array, offset / sizeof(int) + _memory.Offset, length / sizeof(int));
        }

        public void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(int) + _memory.Offset,
                IntPtr.Add(dst.Lock(), dstOffset), length / sizeof(int));
            dst.Unlock();
        }
    }
}