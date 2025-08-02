using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vicold.Atmospex.Data.DataMemory
{
    internal class ByteGridDataMemory : IGridDataMemory
    {
        private ArraySegment<byte> _memory;
        private GCHandle _lockedData;

        public ByteGridDataMemory(byte[] buffer)
        {
            _memory = new ArraySegment<byte>(buffer);
        }

        public ByteGridDataMemory(byte[] buffer, int offset, int length)
        {
            _memory = new ArraySegment<byte>(buffer, offset, length);
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
            return IntPtr.Add(_lockedData.AddrOfPinnedObject(), _memory.Offset);
        }

        public byte ReadByte(int offset)
        {
            return _memory.Array[_memory.Offset + offset];
        }

        public ushort ReadUInt16(int offset)
        {
            return BitConverter.ToUInt16(_memory.Array, offset + _memory.Offset);
        }

        public short ReadInt16(int offset)
        {
            return BitConverter.ToInt16(_memory.Array, offset + _memory.Offset);
        }

        public uint ReadUInt32(int offset)
        {
            return BitConverter.ToUInt32(_memory.Array, offset + _memory.Offset);
        }

        public int ReadInt32(int offset)
        {
            return BitConverter.ToInt32(_memory.Array, offset + _memory.Offset);
        }

        public float ReadFloat(int offset)
        {
            return BitConverter.ToSingle(_memory.Array, offset + _memory.Offset);
        }

        public void Read(int offset, IntPtr dst, int length)
        {
            Marshal.Copy(_memory.Array, offset + _memory.Offset, dst, length);
        }

        public void Unlock()
        {
            if (_lockedData.IsAllocated)
                _lockedData.Free();
        }

        public unsafe void UnsafeAction(Action<IntPtr> action)
        {
            fixed (byte* ptr = _memory.Array)
            {
                action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public unsafe T UnsafeAction<T>(Func<IntPtr, T> action)
        {
            fixed (byte* ptr = _memory.Array)
            {
                return action(new IntPtr(ptr + _memory.Offset));
            }
        }

        public void Write(int offset, IntPtr src, int length)
        {
            Marshal.Copy(src, _memory.Array, offset + _memory.Offset, length);
        }

        public void CopyTo(int offset, IGridDataMemory dst, int dstOffset, int length)
        {
            Marshal.Copy(_memory.Array, offset + _memory.Offset,
                IntPtr.Add(dst.Lock(), dstOffset), length);
            dst.Unlock();
        }
    }
}
