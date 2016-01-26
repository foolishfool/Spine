using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clientTBL
{
    /// <summary>
    /// 泛型单例模式
    /// </summary>
    public class SingletonHolder<T> where T:new()
    {
        static SingletonHolder()
        {
        }
        private static object _lock = new object();  

        public static T Instance
        {
            get 
            {
                lock (_lock)
                {
                    return SingletonInner.singleton;
                }
                
            }
        }

        class SingletonInner
        {
            static SingletonInner() { }
            internal static T singleton = new T();
        }
    }
}
