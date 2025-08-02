using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;

namespace Vicold.Atmospex.DataService.Provider
{
    public static class TypeTool
    {
        public static byte GetTypeSize<TType>() where TType : struct
        {
            return (byte)Marshal.SizeOf<TType>();
        }

        /// <summary>
        /// 获取数据类型的字节数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte GetTypeLength(DataType type)
        {
            switch (type)
            {
                case DataType.Byte: return 1;
                case DataType.Int16: return 2;
                case DataType.Int32: return 4;
                case DataType.Int64: return 8;
                case DataType.Float32: return 4;
                case DataType.Float64: return 8;
            }

            return 0;
        }

        /// <summary>
        /// 根据泛型获取数据类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static DataType GetType<TType>() where TType : struct
        {
            if (typeof(TType) == typeof(byte))
            {
                return DataType.Byte;
            }
            else if (typeof(TType) == typeof(short))
            {
                return DataType.Int16;
            }
            else if (typeof(TType) == typeof(int))
            {
                return DataType.Int32;
            }
            else if (typeof(TType) == typeof(long))
            {
                return DataType.Int64;
            }
            else if (typeof(TType) == typeof(float))
            {
                return DataType.Float32;
            }
            else if (typeof(TType) == typeof(double))
            {
                return DataType.Float64;
            }
            else
            {
                return DataType.Unknown;
            }
        }

        /// <summary>
        /// 判断泛型是否为有效数值数据类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static bool IsNumericType<TType>() where TType : struct
        {
            return GetType<TType>() != DataType.Unknown;
        }

        /// <summary>
        /// 字节数组转数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object GetTypeData(DataType type, byte[] data)
        {
            switch (type)
            {
                case DataType.Byte:
                    return data[0];
                case DataType.Int16:
                    return BitConverter.ToInt16(data, 0);
                case DataType.Int32:
                    return BitConverter.ToInt32(data, 0);
                case DataType.Int64:
                    return BitConverter.ToInt64(data, 0);
                case DataType.Float32:
                    return BitConverter.ToSingle(data, 0);
                case DataType.Float64:
                    return BitConverter.ToDouble(data, 0);
            }

            return 0;
        }

        public static void Transform<TType>(byte[] from, TType[] to) where TType : struct
        {
            var typeSize = GetTypeLength(GetType<TType>());
            var dataLength = from.LongLength / typeSize;

            if (typeof(TType) == typeof(byte))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType((from[i]), typeof(TType));
                }
            }
            else if (typeof(TType) == typeof(short))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType(BitConverter.ToInt16(from, i * typeSize), typeof(TType));
                }
            }
            else if (typeof(TType) == typeof(int))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType(BitConverter.ToInt32(from, i * typeSize), typeof(TType));
                }
            }
            else if (typeof(TType) == typeof(long))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType(BitConverter.ToInt64(from, i * typeSize), typeof(TType));
                }
            }
            else if (typeof(TType) == typeof(float))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType(BitConverter.ToSingle(from, i * typeSize), typeof(TType));
                }
            }
            else if (typeof(TType) == typeof(double))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (TType)Convert.ChangeType(BitConverter.ToDouble(from, i * typeSize), typeof(TType));
                }
            }
        }
        public static void Transform<TType>(TType[] from, byte[] to) where TType : struct
        {
            var typeSize = GetTypeLength(GetType<TType>());
            var dataLength = from.LongLength / typeSize;

            if (typeof(TType) == typeof(byte))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    to[i] = (byte)Convert.ChangeType((from[i]), typeof(byte));
                }
            }
            else if (typeof(TType) == typeof(short))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    var bytes = BitConverter.GetBytes((short)Convert.ChangeType(from[i], typeof(short)));
                    Array.Copy(bytes, 0, to, i * typeSize, typeSize);
                }
            }
            else if (typeof(TType) == typeof(int))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    var bytes = BitConverter.GetBytes((int)Convert.ChangeType(from[i], typeof(int)));
                    Array.Copy(bytes, 0, to, i * typeSize, typeSize);
                }
            }
            else if (typeof(TType) == typeof(long))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    var bytes = BitConverter.GetBytes((long)Convert.ChangeType(from[i], typeof(long)));
                    Array.Copy(bytes, 0, to, i * typeSize, typeSize);
                }
            }
            else if (typeof(TType) == typeof(float))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    var bytes = BitConverter.GetBytes((float)Convert.ChangeType(from[i], typeof(float)));
                    Array.Copy(bytes, 0, to, i * typeSize, typeSize);
                }
            }
            else if (typeof(TType) == typeof(double))
            {
                for (var i = 0; i < dataLength; i++)
                {
                    var bytes = BitConverter.GetBytes((double)Convert.ChangeType(from[i], typeof(double)));
                    Array.Copy(bytes, 0, to, i * typeSize, typeSize);
                }
            }
        }
    }
}
