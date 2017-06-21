using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Microservice.Web.Service
{
    /// <summary>
    /// 基础服务工厂类
    /// th
    /// 2016/8/11
    /// </summary>
    /// <typeparam name="T">服务接口类型</typeparam>
    public class BaseFactory<T> where T : IBaseService
    {
        Type _k;

        static T Instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="k">服务类</param>
        public BaseFactory(Type k)
        {
            _k = k;
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <returns></returns>
        public T GetInstance()
        {
            if (Instance == null)
            {
                Instance = (T)Activator.CreateInstance(_k);
            }
            return Instance;
        }
    }
}