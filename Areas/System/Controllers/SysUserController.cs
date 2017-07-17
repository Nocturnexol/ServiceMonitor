using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BS.Microservice.Web.Model;
using BS.Microservice.Web.Common;
using BS.Common;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    [AllowAnonymous]
    public class SysUserController : Controller
    {
        //
        // GET: /System/SysUser/

        public ActionResult Index()
        {
            ViewBag.BtnList = CommonHelper.GetBtnAuthorityForPage("用户管理"); var query = BusinessContext.tblDepart.GetList();
            List<SelectListItem> DepartList = query.GroupBy(p => p.dept).Select(p => new SelectListItem { Text = p.Key, Value = p.Key }).ToList();
            DepartList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });
            ViewBag.DepartList = DepartList;
            return View();
        }

        public ActionResult Create()
        {
            List<SelectListItem> deptList = BusinessContext.tblDepart.GetList().Select(p => new SelectListItem { Text = p.dept, Value = p.dept }).ToList();
            deptList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });
            ViewData["deptList"] = deptList;

            List<SelectListItem> RoleList = BusinessContext.sys_role.GetList().Select(p => new SelectListItem { Text = p.role_name, Value = p.Rid.ToString() }).ToList();
            RoleList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });

            ViewData["RoleList"] = RoleList;
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserEntity collection, string IsContinue)
        {
            ReturnMessage RM = new ReturnMessage(false);

            try
            {
                if (collection.UserPwd == "" || string.IsNullOrEmpty(collection.UserPwd))
                {
                    //默认密码MD5加密
                    collection.UserPwd = Md5.Encode("123456");
                }
                //根据登录名称查询是否已经存在,
                var query = BusinessContext.User.GetModel(collection.LoginName);
                if (query != null)
                {
                    RM.Message = "登录名已被占用";
                }
                else
                {
                    RM.IsSuccess = BusinessContext.User.Add(collection);
                    if (RM.IsSuccess)
                    {
                       
                        if (IsContinue == "1")
                        {
                            RM.IsContinue = true;
                        }
                        else
                        {
                            RM.IsContinue = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RM.Message = ex.Message;
            }

            return Json(RM);
        }

        public ActionResult Edit(int id)
        {

            UserEntity user = BusinessContext.User.GetModel(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.DefaultRoleId == 0)
            {
                var q = Query.And(Query<tblUser_Roles>.EQ(t => t.LoginName, user.LoginName),
                    Query<tblUser_Roles>.EQ(t => t.IsDefault, true));
                var query = BusinessContext.tblUser_Roles.GetList(q).OrderBy(p => p.Rid).Select(p => p.Role_Id).ToList();
                if (query != null && query.Count > 0)
                {
                    user.DefaultRoleId = query[0];
                }
            }
            List<SelectListItem> deptList = BusinessContext.tblDepart.GetList().Select(p => new SelectListItem { Text = p.dept, Value = p.dept, Selected = user.dept_New == p.dept }).ToList();
            deptList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "" });
            ViewData["deptList"] = deptList;
            List<SelectListItem> RoleList = BusinessContext.sys_role.GetList().Select(p => new SelectListItem { Text = p.role_name, Value = p.Rid.ToString(), Selected = user.DefaultRoleId == p.Rid }).ToList();
            RoleList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "" });
            ViewData["RoleList"] = RoleList;

            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(UserEntity collection)
        {
            ReturnMessage RM = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    bool flag = BusinessContext.User.Exists(collection.LoginName, collection._id);
                    if (flag)
                    {
                        RM.Message = "登录名已被占用";
                    }
                    else
                    {
                        RM.IsSuccess = BusinessContext.User.Update(collection);
                    }
                }
                catch (Exception ex)
                {
                    RM.Message = ex.Message;
                }
            }
            return Json(RM);
        }

        /// <summary>
        /// 获取用户列表，用于jqGrid展示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDataList(string LoginName = null, int page = 1, int rows = 20, string sidx = "_id", string sord = "asc")
        {
            int CurrentPageIndex = (page != 0 ? (int)page : 1);
            sidx = string.IsNullOrWhiteSpace(sidx) ? "_id" : sidx;
            List<UserEntity> list = BusinessContext.User.GetModelList(null, sidx, sord, page, rows);
            int totalCount = BusinessContext.User.GetCount(null);
            JqGridData RM = new JqGridData();
            RM.page = CurrentPageIndex;
            RM.rows = list;
            RM.total = (totalCount % rows == 0 ? totalCount / rows : totalCount / rows + 1);
            RM.records = totalCount;
            return Json(RM, JsonRequestBehavior.AllowGet);
        } 

    }
}
