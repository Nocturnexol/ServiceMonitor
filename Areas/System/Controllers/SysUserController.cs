using System;
using System.Linq;
using System.Web.Mvc;
using BS.Common;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
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
            var departList = query.GroupBy(p => p.dept).Select(p => new SelectListItem { Text = p.Key, Value = p.Key }).ToList();
            departList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });
            ViewBag.DepartList = departList;
            return View();
        }

        public ActionResult Create()
        {
            var deptList = BusinessContext.tblDepart.GetList().Select(p => new SelectListItem { Text = p.dept, Value = p.dept }).ToList();
            deptList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });
            ViewData["deptList"] = deptList;

            var roleList = BusinessContext.sys_role.GetList().Select(p => new SelectListItem { Text = p.role_name, Value = p.Rid.ToString() }).ToList();
            roleList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "", Selected = true });

            ViewData["RoleList"] = roleList;
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserEntity collection, string isContinue)
        {
            var rm = new ReturnMessage(false);

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
                    rm.Message = "登录名已被占用";
                }
                else
                {
                    rm.IsSuccess = BusinessContext.User.Add(collection);
                    if (rm.IsSuccess)
                    {
                        rm.IsContinue = isContinue == "1";
                    }
                }
            }
            catch (Exception ex)
            {
                rm.Message = ex.Message;
            }

            return Json(rm);
        }

        public ActionResult Edit(int id)
        {

            var user = BusinessContext.User.GetModel(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.DefaultRoleId == 0)
            {
                var q = Query.And(Query<tblUser_Roles>.EQ(t => t.LoginName, user.LoginName),
                    Query<tblUser_Roles>.EQ(t => t.IsDefault, true));
                var query = BusinessContext.tblUser_Roles.GetList(q).OrderBy(p => p.Rid).Select(p => p.Role_Id).ToList();
                if (query.Count > 0)
                {
                    user.DefaultRoleId = query[0];
                }
            }
            var deptList = BusinessContext.tblDepart.GetList().Select(p => new SelectListItem { Text = p.dept, Value = p.dept, Selected = user.dept_New == p.dept }).ToList();
            deptList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "" });
            ViewData["deptList"] = deptList;
            var roleList = BusinessContext.sys_role.GetList().Select(p => new SelectListItem { Text = p.role_name, Value = p.Rid.ToString(), Selected = user.DefaultRoleId == p.Rid }).ToList();
            roleList.Insert(0, new SelectListItem { Text = "-请选择-", Value = "" });
            ViewData["RoleList"] = roleList;

            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(UserEntity collection)
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    var flag = BusinessContext.User.Exists(collection.LoginName, collection._id);
                    if (flag)
                    {
                        rm.Message = "登录名已被占用";
                    }
                    else
                    {
                        rm.IsSuccess = BusinessContext.User.Update(collection);
                    }
                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            return Json(rm);
        }

        /// <summary>
        /// 获取用户列表，用于jqGrid展示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDataList(int page = 1, int rows = 20, string sidx = "_id", string sord = "asc")
        {
            int currentPageIndex = page != 0 ? page : 1;
            sidx = string.IsNullOrWhiteSpace(sidx) ? "_id" : sidx;
            var list = BusinessContext.User.GetModelList(null, sidx, sord, page, rows);
            var totalCount = BusinessContext.User.GetCount(null);
            var rm = new JqGridData();
            rm.page = currentPageIndex;
            rm.rows = list;
            rm.total = totalCount % rows == 0 ? totalCount / rows : totalCount / rows + 1;
            rm.records = totalCount;
            return Json(rm, JsonRequestBehavior.AllowGet);
        } 

    }
}
