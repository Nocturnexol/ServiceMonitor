using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BS.Microservice.Web.BLL;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class FunctionalController : Controller
    {
        private readonly MongoBll<FunctionalAuthority> _bll = new MongoBll<FunctionalAuthority>();
        //
        // GET: /System/Functional/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            //取得按钮权限
            var btnList = CommonHelper.GetBtnAuthorityForPage("数据权限");
            ViewBag.BtnList = btnList;
            return View();
        }
        public ActionResult GetFunctionalDataList(string Module_Name = "", string Group_Name = "", string Right_Name = "", int page = 1, int rows = 20, string sidx = "", string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "Rid";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            List<IMongoQuery> queryList = new List<IMongoQuery>();

            if (!string.IsNullOrWhiteSpace(Module_Name))
            {
                queryList.Add(Query<FunctionalAuthority>.Matches(t => t.Module_Name,
                        new BsonRegularExpression(new Regex(Module_Name, RegexOptions.IgnoreCase))));
            }
            if (!string.IsNullOrWhiteSpace(Group_Name))
            {
                queryList.Add(Query<FunctionalAuthority>.Matches(t => t.Group_Name,
                         new BsonRegularExpression(new Regex(Group_Name, RegexOptions.IgnoreCase))));
            }
            if (!string.IsNullOrWhiteSpace(Right_Name))
            {
                queryList.Add(Query<FunctionalAuthority>.Matches(t => t.Right_Name,
                        new BsonRegularExpression(new Regex(Right_Name, RegexOptions.IgnoreCase))));
            }

            var where = queryList.Any() ? Query.And(queryList) : null;

            long totalCount;
            //string orderBy = "module_id,group_id,right_Id";
            var rm = new JqGridData
            {
                page = pager.CurrentPageIndex,
                rows = _bll.GetList(out totalCount, page, rows, where, sidx, sord),
                total = (int)(totalCount % pager.PageSize == 0 ? totalCount / pager.PageSize : totalCount / pager.PageSize + 1),
                records = (int)totalCount
            };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /System/Functional/Create

        [HttpPost]
        public ActionResult Create(FunctionalAuthority obj, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    //var list = BusinessContext.FunctionalAuthority.GetList("Module_Name ='" + obj.Module_Name + "'");
                    var list =
                        BusinessContext.FunctionalAuthority.GetList(Query<FunctionalAuthority>.EQ(t => t.Module_Name,
                            obj.Module_Name));
                    //var list = BusinessContext.FunctionalAuthority.Where(p => p.Module_Name == obj.Module_Name).ToList();
                    if (list != null && list.Count > 0)
                    {
                        obj.Module_Id = list.First().Module_Id;
                        var group = list.FirstOrDefault(p => p.Group_Name == obj.Group_Name);
                        var right = list.FirstOrDefault(p => p.Right_Name == obj.Right_Name);
                        if (right != null)
                        {
                            rm.Message = "页面名称重复";
                            return Json(rm);

                        }
                        if (group == null)
                        {
                            obj.Group_Id = list.Max(p => p.Group_Id) + 1;
                            obj.Right_Id = 1;
                        }
                        else
                        {
                            obj.Group_Id = group.Group_Id;
                            obj.Right_Id = Convert.ToInt32(list.Where(p => p.Group_Id == obj.Group_Id).Max(p => p.Right_Id)) + 1;
                        }
                    }
                    else
                    {
                        int maxModule = Convert.ToInt32(BusinessContext.FunctionalAuthority.GetMaxId());
                        obj.Module_Id = maxModule + 1;
                        obj.Group_Id = 1;
                        obj.Right_Id = 1;
                    }
                    var res = BusinessContext.FunctionalAuthority.Add(obj);
                    rm.IsSuccess = res;
                    if (rm.IsSuccess)
                    {
                        rm.IsContinue = isContinue == "1";
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
            var dpt = BusinessContext.FunctionalAuthority.Get(Query<FunctionalAuthority>.EQ(t => t.Rid, id));
            if (dpt == null)
            {
                return HttpNotFound();
            }
            return View(dpt);
        }
        [HttpPost]
        public ActionResult Edit(FunctionalAuthority collection)
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    rm.IsSuccess = BusinessContext.FunctionalAuthority.Update(collection);
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
                string paramData = Request.Form["paramData"];
                if (!string.IsNullOrWhiteSpace(paramData))
                {
                    var RidArr = paramData.Split('*').Select(int.Parse);
                    var ridArr = RidArr as IList<int> ?? RidArr.ToList();
                    var res = _bll.Delete(ridArr);
                    //StringBuilder strSql = new StringBuilder(" delete from FunctionalAuthority where Rid in " + DBContext.DataDecision.AssemblyInCondition(ridArr.ToList()));
                    //lsSql.Add(strSql.ToString());
                    //List<FunctionalAuthority> F_modelList = BusinessContext.FunctionalAuthority.GetList(Query<FunctionalAuthority>.In(t => t.Rid, ridArr)).ToList();

                    var rids =
                        BusinessContext.tblGroupButton.GetList(Query<tblGroupButton>.In(t => t.Group_NameId, ridArr))
                            .Select(t => t.Rid)
                            .ToList();

                    res = res &&
                          BusinessContext.sys_role_right.Delete(Query<sys_role_right>.In(t => t.rf_Right_Code, rids));
                    //strSql = new StringBuilder("delete from dbo.sys_role_right where rf_right_code in (select Rid  from dbo.tblGroupButton where Group_NameId in " + DBContext.DataDecision.AssemblyInCondition(ridArr.ToList()) + ")");
                    //lsSql.Add(strSql.ToString());
                    //List<sys_role_right> S_modelList = BusinessContext.sys_role_right.GetModelList("rf_right_code in (select Rid  from dbo.tblGroupButton where Group_NameId in " + DBContext.DataDecision.AssemblyInCondition(ridArr.ToList()) + ")").ToList();


                    res = res && BusinessContext.tblGroupButton.Delete(rids);
                    //strSql = new StringBuilder("delete from dbo.tblGroupButton where Group_NameId in " + DBContext.DataDecision.AssemblyInCondition(ridArr.ToList()));
                    //lsSql.Add(strSql.ToString());
                    //List<tblGroupButton> G_modelList = BusinessContext.tblGroupButton.GetModelList("Group_NameId in " + DBContext.DataDecision.AssemblyInCondition(ridArr.ToList())).ToList();

                    if (res)
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
