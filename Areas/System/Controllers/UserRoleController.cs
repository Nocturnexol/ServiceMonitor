using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class UserRoleController : Controller
    {
        //
        // GET: /System/UserRole/
        /// <summary>
        /// 人员角色设置
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public ActionResult Index(string loginName)
        {
            ViewBag.LoginName = loginName;

            var roleIList = BusinessContext.sys_role.GetList();
            //当前用户的所有角色
            var rolesList =
                BusinessContext.tblUser_Roles.GetList(Query<tblUser_Roles>.EQ(t => t.LoginName, loginName));
            var rightInfos = rolesList.Select(roleRight => roleRight.LoginName + "|" + roleRight.Role_Id + "|" + roleRight.Remark).ToList();
            ViewBag.RoleList = rightInfos;
            return View(roleIList);
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData()
        {
            var rm = new ReturnMessage();
            try
            {
                var paramData = Request.Form["paramData"];
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tblUser_Roles>>(paramData);
                rm.IsSuccess = BusinessContext.tblUser_Roles.Add(list);
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
