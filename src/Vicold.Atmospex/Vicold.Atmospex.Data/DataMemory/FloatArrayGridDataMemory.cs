using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vicold.Atmospex.Data.DataMemory
{
    internal class FloatArrayGridDataMemory : IGridDataMemory
    {
        private GCHandle _lockedData;
        private ArraySegment<float> _memory;

        public FloatArrayGridDataMemory(float[] buffer)
        {
            _memory = new ArraySegment<float>(buffer);
        }

        public FloatArrayGridDataMemory(float[] buffer, int offset, int length)
        {
            _memory = new ArraySegment<float>(buffer, offset, length);
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
            return IntPtr.Add(_lockedData.AddrOfPinnedObject(), _memory.Offset * sizeof(float));
        }

        public unsafe byte ReadByte(int offset)
        {
            fixed (float* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *(pb + _memory.Offset * sizeof(float) + offset);
            }
        }

        public unsafe ushort ReadUInt16(int offset)
        {
            fixed (float* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((ushort*)(pb + _memory.Offset * sizeof(float) + offset));
            }
        }

        public unsafe short ReadInt16(int offset)
        {
            fixed (float* ptr = _memory.Array)
            {
                byte* pb = (byte*)ptr;
                return *((short*)(pb + _memory.Offset * sizeof(float) + offset));
            }
        }

        public uint ReadUInt32(int offset)
        {
            return Convert.ToUInt32(_memory.Array[_memory.Offset + offset / sizeof(float)]);
        }

        public int ReadInt32(int offset)
        {
            return Convert.ToInt32(_memory.Array[_memory.Offset + offset / sizeof(float)]);
        }

        public float ReadFloat(int offset)
        {
            return _memory.Array[_memory.Offset + offset / sizeof(float)];
        }

        public void Read(int offset, IntPtr dst, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(float) + _memory.Offset, dst, length / sizeof(float));
        }

        public void Unlock()
        {
            if (_lockedData.IsAllocated)
                _lockedData.Free();
        }

        public unsafe void UnsafeAction(Action<IntPtr> action)
        {
            fixed (float* ptr = _memory.Array)
            {
                action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public unsafe T UnsafeAction<T>(Func<IntPtr, T> action)
        {
            fixed (float* ptr = _memory.Array)
            {
                return action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public void Write(int offset, IntPtr src, int length)
        {
            Marshal.Copy(src, _memory.Array, offset / sizeof(float) + _memory.Offset, length / sizeof(float));
        }

        public void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length)
        {
            Marshal.Copy(_memory.Array, offset / sizeof(float) + _memory.Offset,
                IntPtr.Add(dst.Lock(), dstOffset), length / sizeof(float));
            dst.Unlock();
        }
    }
}