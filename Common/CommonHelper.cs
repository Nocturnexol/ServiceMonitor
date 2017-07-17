using System;
using System.Collections.Generic;
using System.Linq;
using BS.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver.Builders;

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

            //List<FunctionalAuthority> list = new List<FunctionalAuthority>();
            //list.Add(new FunctionalAuthority
            //{
            //    Group_Id = 1,
            //    Group_Name = "系统设置",
            //    Module_Id = 999,
            //    Module_Name = "系统设置",
            //    Right_Id = 1,
            //    Right_Name = "用户管理",
            //    Rigth_Url = "/System/SysUser/Index"
            //});
            //list.Add(new FunctionalAuthority
            //{
            //    Group_Id = 2,
            //    Group_Name = "服务管理",
            //    Module_Id = 998,
            //    Module_Name = "服务管理",
            //    Right_Id = 2,
            //    Right_Name = "服务管理",
            //    Rigth_Url = "/Service/Manage/Index?type=1"
            //});
            //list.Add(new FunctionalAuthority
            //{
            //    Group_Id = 2,
            //    Group_Name = "服务管理",
            //    Module_Id = 998,
            //    Module_Name = "服务管理",
            //    Right_Id = 3,
            //    Right_Name = "联系人",
            //    Rigth_Url = "/Service/Monitor/Index"
            //});
            //return list;
            List<FunctionalAuthority> list = new List<FunctionalAuthority>();
            try
            {
                List<int> dic_roleId = CurrentHelper.CurrentUser.Roles.Select(p => p.Rid).ToList();
                var q = Query.And(Query<sys_role_right>.In(t => t.rf_Role_Id, dic_roleId),
                    Query<sys_role_right>.EQ(t => t.rf_Type, "数据管理"));
                List<int> rigthCodeList =
                    BusinessContext.sys_role_right.GetList(q).Select(p => p.rf_Right_Code).ToList();
                if (rigthCodeList != null && rigthCodeList.Count > 0)
                {
                    List<int> dic_funId = BusinessContext.tblGroupButton.GetList(Query<tblGroupButton>.In(t => t.Rid, rigthCodeList)).Select(p => p.Group_NameId).ToList();
                    list = BusinessContext.FunctionalAuthority.GetList(Query<FunctionalAuthority>.In(t => t.Rid, dic_funId));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("GetFunctionalAuthoirty", ex);
            }
            return list;
        }


        /// <summary>
        /// 获取当前用户页面按钮权限
        /// </summary>
        /// <param name="Func">页面功能名</param>
        /// <returns></returns>
        public static List<string> GetBtnAuthorityForPage(string Func)
        {
            var list = new List<string>();
            try
            {
                List<int> listRoleId = CurrentHelper.CurrentUser.Roles.Select(p => p.Rid).ToList();
                // 获取业务权限对象
                List<int> ridlist = BusinessContext.FunctionalAuthority.GetList(Query<FunctionalAuthority>.EQ(t => t.Right_Name, Func)).Select(p => p.Rid).ToList();

                // 获取用户所有的数据权限编码列表
                List<int> rigthCodeList = BusinessContext.sys_role_right.GetList(Query.And(Query<sys_role_right>.In(t => t.rf_Role_Id, listRoleId),
                    Query<sys_role_right>.EQ(t => t.rf_Type, "数据管理"))).Select(p => p.rf_Right_Code).Distinct().ToList();

                if (ridlist.Count > 0 && rigthCodeList.Count > 0)
                {
                    list = (from gb in BusinessContext.tblGroupButton.GetList()
                            join bn in BusinessContext.tblButtonName.GetList() on gb.ButtonNameId equals bn.Rid into g
                            from a in g.DefaultIfEmpty()
                            join f in BusinessContext.FunctionalAuthority.GetList() on gb.Group_NameId equals f.Rid into gg
                            from aa in gg.DefaultIfEmpty()
                            where ridlist.Contains(gb.Group_NameId) && rigthCodeList.Contains(gb.Rid)
                            select a.ButtonName).ToList();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("GetBtnAuthorityForPage", ex);
                throw;
            }
            return list;
        }

    }
}