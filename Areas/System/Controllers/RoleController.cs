using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /System/Role/

        public ActionResult Index()
        {
            //取得按钮权限
            var btnList = CommonHelper.GetBtnAuthorityForPage("角色管理");
            ViewBag.BtnList = btnList;
            return View();
        }

        public ActionResult GetRoleDataList(int page = 1, int rows = 20, string keyword = null, string sidx = "", string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "Rid";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            var queryList = new List<IMongoQuery>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryList.Add(Query<sys_role>.Matches(t => t.role_name,
                    new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))));
            }
            var where = queryList.Any() ? Query.And(queryList) : null;

            long totalCount;
            var rm = new JqGridData
            {
                page = pager.CurrentPageIndex,
                rows = BusinessContext.sys_role.GetList(out totalCount, page, rows, where, sidx, sord),
                total = (int)(totalCount % pager.PageSize == 0 ? totalCount / pager.PageSize : totalCount / pager.PageSize + 1),
                records = (int)totalCount
            };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(sys_role collection, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    var query =
                        BusinessContext.sys_role.GetList(Query<sys_role>.EQ(t => t.role_name, collection.role_name));
                    if (query.Count > 0)
                    {
                        rm.Message = "角色名称已被占用";
                    }
                    else
                    {
                        collection.CreateOn = DateTime.Now;
                        collection.CreateBy = CurrentHelper.CurrentUser.User.UserName;
                        var res = BusinessContext.sys_role.Add(collection);
                        rm.IsSuccess = res;
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
            }
            return Json(rm);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var dpt = BusinessContext.sys_role.Get(Query<sys_role>.EQ(t => t.Rid, id));
            if (dpt == null)
            {
                return HttpNotFound();
            }
            return View(dpt);
        }
        [HttpPost]
        public ActionResult Edit(sys_role collection)
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    var q = Query.And(Query<sys_role>.EQ(t => t.role_name, collection.role_name),
                        Query<sys_role>.NE(t => t.Rid, collection.Rid));
                    var query = BusinessContext.sys_role.GetList(q);
                    if (query.Count > 0)
                    {
                        rm.Message = "角色名称已被占用";
                    }
                    else
                    {
                        collection.ModifyOn = DateTime.Now;
                        collection.ModifyBy = CurrentHelper.CurrentUser.User.UserName;
                        rm.IsSuccess = BusinessContext.sys_role.Update(collection);
                    }
                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            return Json(rm);
        }
        [HttpPost]
        public ActionResult DataDel()
        {
            var rm = new ReturnMessage();
            try
            {
                var paramData = Request.Form["paramData"];
                var ridArr = paramData.Split('*').Select(int.Parse).ToList();
                var roles = BusinessContext.tblUser_Roles.GetList(Query<tblUser_Roles>.In(t => t.Role_Id, ridArr));
                //StringBuilder strSql = new StringBuilder(" select Role_Id from tblUser_Roles where Role_Id in " + DBContext.AssemblyInCondition(RidArr.ToList()));
                //DataTable dt = DBContext.DataDecision.GetDataTable(strSql.ToString());
                if (roles.Any())
                {
                    rm.IsSuccess = false;
                    rm.Message = "存在用户拥有此角色，无法删除！";
                }
                else
                {
                    if (BusinessContext.sys_role.Delete(ridArr))
                    {

                        rm.IsSuccess = true;
                        rm.Message = "删除成功！";
                    }
                    else
                    {
                        rm.IsSuccess = false;
                        rm.Message = "删除失败！";
                    }
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
