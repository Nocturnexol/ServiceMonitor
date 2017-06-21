using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Microservice.Web.Model;

namespace BS.Microservice.Web.Common
{
    public class CommonHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<FunctionalAuthority> GetFunctionalAuthoirty()
        {

            List<FunctionalAuthority> list = new List<FunctionalAuthority>();
            list.Add(new FunctionalAuthority()
            {
                Group_Id = 1,
                Group_Name = "系统设置",
                Module_Id = 999,
                Module_Name = "系统设置",
                Right_Id = 1,
                Right_Name = "用户管理",
                Rigth_Url = "/System/SysUser/Index"
            });
            list.Add(new FunctionalAuthority()
            {
                Group_Id = 2,
                Group_Name = "服务管理",
                Module_Id = 998,
                Module_Name = "服务管理",
                Right_Id = 2,
                Right_Name = "服务管理",
                Rigth_Url = "/Service/Manage/Index"
            });
            list.Add(new FunctionalAuthority()
            {
                Group_Id = 2,
                Group_Name = "服务管理",
                Module_Id = 998,
                Module_Name = "服务管理",
                Right_Id = 3,
                Right_Name = "联系人",
                Rigth_Url = "/Service/Monitor/Index"
            });
            return list;
        }
    }
}