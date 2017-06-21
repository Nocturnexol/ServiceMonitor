using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Microservice.Web.BLL;

namespace BS.Microservice.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessContext
    {
        /// <summary>
        /// User业务处理类
        /// </summary>
        public static UserBLL User
        {
            get
            {
                if (_User == null)
                {
                    _User = new UserBLL();
                }
                return _User;
            }
        }
        private static UserBLL _User;


        /// <summary>
        /// ServiceList业务处理类
        /// </summary>
        public static ServiceListBLL ServiceList
        {
            get
            {
                if (_ServiceList == null)
                {
                    _ServiceList = new ServiceListBLL();
                }
                return _ServiceList;
            }
        }
        private static ServiceListBLL _ServiceList;
    }
}