//using Vicold.Atmospex.Core.Bus;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;

//namespace Vicold.Atmospex.Core.Core
//{
//    internal sealed class GlobalBus : IBus
//    {

//        #region 单例模式

//        private GlobalBus()
//        {
//            _transportHolder = new ConcurrentDictionary<string, ITransport>();
//        }

//        private static class GlobalBusHolder
//        {
//            internal static readonly GlobalBus INSTANCE = new GlobalBus();
//        }

//        public static IBus Current => GlobalBusHolder.INSTANCE;

//        #endregion

//        ConcurrentDictionary<string, ITransport> _transportHolder;


//        public ITransport GetTransport(string name)
//        {
//            if (_transportHolder.TryGetValue(name, out var instance))
//            {
//                return instance;
//            }

//            return null;
//        }

//        public T GetTransport<T>() where T : ITransport
//        {
//            var tType = typeof(T);
//            if (_transportHolder.TryGetValue(tType.Name, out var instance))
//            {
//                return (T)instance;
//            }

//            return default;
//        }

//        public T RegisterTransport<IT, T>() where T : IT
//        {
//            var itType = typeof(IT);
//            var tType = typeof(T);
//            var transportType = typeof(ITransport);
//            if (!transportType.IsAssignableFrom(tType))
//            {
//                throw new Exception("T需实现ITransport接口");
//            }

//            var instance = (ITransport)Activator.CreateInstance(tType);
//            if (_transportHolder.TryGetValue(itType.Name, out _))
//            {
//                throw new Exception("ITransport对象多次载入");
//            }

//            instance.LoadBus(this);
//            _transportHolder[itType.Name] = instance;
//            return (T)instance;
//        }

//        public T RegisterTransport<T>()
//        {
//            var tType = typeof(T);
//            var transportType = typeof(ITransport);
//            if (!transportType.IsAssignableFrom(tType))
//            {
//                throw new Exception("T需实现ITransport接口");
//            }

//            var instance = (ITransport)Activator.CreateInstance(tType);
//            if (_transportHolder.TryGetValue(tType.Name, out _))
//            {
//                throw new Exception("ITransport对象多次载入");
//            }

//            instance.LoadBus(this);
//            _transportHolder[tType.Name] = instance;
//            return (T)instance;
//        }


//        //public T GetTransport<T>() where T : ITransport
//        //{
//        //    var tType = typeof(T);
//        //    if (_transportHolder.TryGetValue(tType.Name, out var instance))
//        //    {
//        //        return (T)instance;
//        //    }
//        //    else
//        //    {
//        //        var newInstance = (ITransport)Activator.CreateInstance(tType);
//        //        _transportHolder[tType.Name] = newInstance;
//        //        return (T)newInstance;
//        //    }
//        //}

//        //public void RegisterTransport<T>(T t)
//        //{

//        //}
//    }
//}
