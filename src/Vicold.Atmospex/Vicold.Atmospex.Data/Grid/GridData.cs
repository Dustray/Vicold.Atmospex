using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;

namespace Vicold.Atmospex.DataService.Provider
{
    public class GridData : IGridData
    {
        protected IGridDataMemory _memory;

        internal GridData(int xsize, int ysize, byte sizeOfType)
        {
            Width = xsize;
            Height = ysize;
            SizeOfType = sizeOfType;
            _memory = new ByteGridDataMemory(new byte[xsize * ysize * SizeOfType]);
        }

        public void ScopeLock(Action<IntPtr> action)
        {
            _memory.UnsafeAction(action);
        }

        public int Width
        {
            get; private set;
        }

        public int Height
        {
            get; private set;
        }

        public GridRegion? ValidSubRegion
        {
            get; set;
        }

        public int SizeOfType
        {
            get; private set;
        }

        public DataType DataType
        {
            get; protected set;
        }

        public double? NoDataValue
        {
            get;
            set;
        }

        public double MaxValue
        {
            get;
            set;
        }

        public double MinValue
        {
            get;
            set;
        }


        unsafe public void FlipVertical()
        {
            _memory.UnsafeAction(ptr =>
            {
                byte* top = (byte*)ptr.ToPointer();
                int rowSize = Width * SizeOfType;
                flipDataVertical(rowSize, rowSize, top, Height);
            });
        }

        #region read and write

        unsafe private void flipDataVertical(int rowSize, int rowStep, byte* top, int height)
        {
            byte* bottom = top + (height - 1) * rowStep;
            while (top < bottom)
            {
                byte* pt = top;
                byte* pb = bottom;

                for (int i = 0; i < rowSize; ++i, ++pt, ++pb)
                {
                    byte temp = *pt;
                    *pt = *pb;
                    *pb = temp;
                }
                top += rowStep;
                bottom -= rowStep;
            }
        }

        public void WriteBlockReverse(int xoffset, int yoffset, int xsize, int ysize, IntPtr data)
        {
            int dstStride = Width * SizeOfType;
            int srcStride = xsize * SizeOfType;

            int dstIndex = yoffset * srcStride + xoffset * SizeOfType;
            for (int row = ysize - 1; row >= 0; --row)
            {
                var srcData = IntPtr.Add(data, row * srcStride);
                _memory.Write(dstIndex, srcData, srcStride);
                dstIndex += dstStride;
            }
        }

        internal unsafe byte ReadBitLittenEndian(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                var b = *((byte*)ptr.ToPointer() + (index >> 3));
                return (byte)(b & (1 << (index & 7)));
            });
        }

        internal unsafe byte ReadBitBigEndian(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                var b = *((byte*)ptr.ToPointer() + (index >> 3));
                return (byte)(b & (1 << (~index & 7)));
            });
        }

        public byte ReadByte(int x, int y)
        {
            int index = y * Width + x;
            return _memory.ReadByte(index);
        }

        public byte ReadByte(int index)
        {
            return _memory.ReadByte(index);
        }

        public unsafe void WriteByte(int x, int y, byte v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((byte*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteByte(int index, byte v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((byte*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe ushort ReadUShort(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                return *((ushort*)ptr.ToPointer() + index);
            });
        }

        public unsafe ushort ReadUShort(int index)
        {
            return _memory.UnsafeAction(ptr =>
            {
                return *((ushort*)ptr.ToPointer() + index);
            });
        }

        public unsafe void WriteUShort(int x, int y, ushort v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((ushort*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteUShort(int index, ushort v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((ushort*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe short ReadShort(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                return *((short*)ptr.ToPointer() + index);
            });
        }

        public unsafe short ReadShort(int index)
        {
            return _memory.UnsafeAction(ptr =>
            {
                return *((short*)ptr.ToPointer() + index);
            });
        }

        public unsafe void WriteShort(int x, int y, short v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((short*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteShort(int index, short v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((short*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe uint ReadUInt(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                return *((uint*)ptr.ToPointer() + index);
            });
        }

        public unsafe uint ReadUInt(int index)
        {
            return _memory.UnsafeAction(ptr =>
            {
                return *((uint*)ptr.ToPointer() + index);
            });
        }

        public unsafe void WriteUInt(int x, int y, uint v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((uint*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteUInt(int index, uint v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((uint*)ptr.ToPointer() + index) = v;
            });
        }

        public int ReadInt(int x, int y)
        {
            int index = y * Width + x;
            return _memory.ReadInt32(index * SizeOfType);
        }

        public int ReadInt(int index)
        {
            return _memory.ReadInt32(index * SizeOfType);
        }

        public unsafe void WriteInt(int x, int y, int v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((int*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteInt(int index, int v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((int*)ptr.ToPointer() + index) = v;
            });
        }

        public float ReadFloat(int x, int y)
        {
            int index = y * Width + x;
            return _memory.ReadFloat(index * SizeOfType);
        }

        public float ReadFloat(int index)
        {
            return _memory.ReadFloat(index * SizeOfType);
        }

        public unsafe void WriteFloat(int x, int y, float v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((float*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteFloat(int index, float v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((float*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe double ReadDouble(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                return *((double*)ptr.ToPointer() + index);
            });
        }

        public unsafe double ReadDouble(int index)
        {
            return _memory.UnsafeAction(ptr =>
            {
                return *((double*)ptr.ToPointer() + index);
            });
        }

        public unsafe void WriteDouble(int x, int y, double v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((double*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteDouble(int index, double v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((double*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe long ReadLong(int x, int y)
        {
            int index = y * Width + x;
            return _memory.UnsafeAction(ptr =>
            {
                return *((long*)ptr.ToPointer() + index);
            });
        }

        public unsafe long ReadLong(int index)
        {
            return _memory.UnsafeAction(ptr =>
            {
                return *((long*)ptr.ToPointer() + index);
            });
        }

        public unsafe void WriteLong(int x, int y, long v)
        {
            int index = y * Width + x;
            _memory.UnsafeAction(ptr =>
            {
                *((long*)ptr.ToPointer() + index) = v;
            });
        }

        public unsafe void WriteLong(int index, long v)
        {
            _memory.UnsafeAction(ptr =>
            {
                *((long*)ptr.ToPointer() + index) = v;
            });
        }

        #endregion

        object ICloneable.Clone() => DeepCopy();

        public void CopyTo(int srcX, int srcY, GridData dst, int dstX, int dstY, int xSize, int ySize)
        {
            if (dst.DataType != DataType)
                throw new InvalidOperationException("Can't copy grid data between different type");

            int dst_stride = dst.Width * dst.SizeOfType;
            int src_stride = Width * SizeOfType;

            int dstIndex = dstY * dst_stride + dstX * SizeOfType;
            int srcIndex = srcY * src_stride + srcX * SizeOfType;

            int copyStride = xSize * SizeOfType;
            for (int i = 0; i < ySize; i++)
            {
                _memory.CopyTo(srcIndex, dst._memory, dstIndex, copyStride);
                srcIndex += src_stride;
                dstIndex += dst_stride;
            }
        }

        public virtual GridData DeepCopy()
        {
            return null;
        }

        protected virtual void _Disposing(bool disposing)
        {
            if (disposing)
            {
                _memory.Dispose();
            }
        }

        public void Dispose()
        {
            this._Disposing(true);
        }
    }
}