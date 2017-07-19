using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class Sys_role_rightController : Controller
    {
        //
        // GET: /System/Sys_role_right/
        public ActionResult Index(int roleId, string roleName)
        {
            var model = (from gb in BusinessContext.tblGroupButton.GetList()
                         join bn in BusinessContext.tblButtonName.GetList() on gb.ButtonNameId equals bn.Rid into g
                         from a in g.DefaultIfEmpty()
                         join f in BusinessContext.FunctionalAuthority.GetList() on gb.Group_NameId equals f.Rid into gg
                         from aa in gg.DefaultIfEmpty()
                         where gb.ButtonNameId != null
                         select new View_GroupButtonInfo
                         {
                             ButtonNameId = gb.ButtonNameId,
                             ButtonName = a.ButtonName,
                             Group_NameId = gb.Group_NameId,
                             Group_Name = aa.Group_Name,
                             Rid = gb.Rid,
                             Module_Id = aa.Module_Id,
                             Module_Name = aa.Module_Name,
                             Right_Name = aa.Right_Name
                         }).ToList();
            //List<View_GroupButtonInfo> GBIList = BusinessContext.View_GroupButtonInfo.GetModelList(" ButtonNameId is not null"); //db.View_GroupButtonInfo.Where(p => p.ButtonNameId != null).ToList();//查找所有模块页面按钮
            //List<OperationalAuthority> OpAList = BusinessContext.OperationalAuthority.GetModelList(" OperRational_Name is not null").ToList();//查找所有的业务权限信息

            ViewBag.RoleId = roleId;
            ViewBag.roleName = roleName;
            var sysRoleRights = BusinessContext.sys_role_right.GetList(Query<sys_role_right>.EQ(t => t.rf_Role_Id, roleId)); //db.sys_role_right.Where(p => p.rf_Role_Id == roleId).ToList();//查找对应角色的所有权限
            var roleRights = sysRoleRights.Where(p => p.rf_Type == "数据管理").ToList();//查找数据权限
            var roleBussiness = sysRoleRights.Where(p => p.rf_Type == "业务权限").ToList();//查找业务权限


            var rightInfos = roleRights.Select(roleRight => roleRight.rf_Type + "|" + roleRight.rf_Role_Id + "|" + roleRight.rf_Right_Code + "|" + roleRight.rf_Right_Authority).ToList();//数据权限拼接
            var rightussinesss = roleBussiness.Select(roleRight => roleRight.rf_Type + "|" + roleRight.rf_Role_Id + "|" + roleRight.rf_Right_Code + "|" + roleRight.rf_Right_Authority).ToList();//数据权限拼接

            //ViewBag.OpAList = OpAList;

            ViewBag.RoleRightList = rightInfos;
            ViewBag.RoleBussinessList = rightussinesss;

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveData()
        {
            var rm = new ReturnMessage();
            try
            {
                var paramData = Request.Form["paramData"];
                var roleId = Convert.ToInt32(Request.Form["RoleId"]);
                List<sys_role_right> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<sys_role_right>>(paramData);

                var oldlist =
                    BusinessContext.sys_role_right.GetList(Query<sys_role_right>.EQ(t => t.rf_Role_Id, roleId));

                var newList = list.Except(oldlist).ToList();//新增的权限
                var delList = oldlist.Except(list).ToList();//删除的权限
                if (newList.Count == 0 && delList.Count == 0)
                {
                    rm.IsSuccess = true;
                    rm.Message = "无权限变化";
                }
                else
                {
                    rm.IsSuccess = BusinessContext.sys_role_right.Delete(delList.Select(t => t.Rid).ToList());
                    rm.IsSuccess = rm.IsSuccess && BusinessContext.sys_role_right.Add(newList);
                }
            }
            catch (Exception ex)
            {
                rm.IsSuccess = false;
                rm.Message = ex.Message;
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

    }
}
