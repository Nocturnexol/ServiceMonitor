using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.System.Controllers
{
    public class DepartController : Controller
    {
        //
        // GET: /System/Depart/

        public ActionResult Index()
        {
            var btnList = CommonHelper.GetBtnAuthorityForPage("部门管理");
            ViewBag.BtnList = btnList;
            return View();
        }

        public ActionResult GetGqDataList(int page = 1, int rows = 20, string keyword = null, string sidx = "dept", string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "dept";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            List<IMongoQuery> queryList = new List<IMongoQuery>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryList.Add(Query<tblDepart>.Matches(t => t.dept,
                    new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))));
            }
            var where = queryList.Any() ? Query.And(queryList) : null;

            long totalCount;
            var rm = new JqGridData
            {
                page = pager.CurrentPageIndex,
                rows = BusinessContext.tblDepart.GetList(out totalCount, page, rows, where, sidx, sord),
                total = (int)(totalCount % pager.PageSize == 0 ? totalCount / pager.PageSize : totalCount / pager.PageSize + 1),
                records = (int)totalCount
            };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /System/Depart/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /System/Depart/Create

        [HttpPost]
        public ActionResult Create(tblDepart collection, string IsContinue = "0")
        {
            ReturnMessage RM = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(collection.dept))
                    {
                        var query = BusinessContext.tblDepart.GetList(Query<tblDepart>.EQ(t => t.dept, collection.dept));
                        if (query.Count > 0)
                        {
                            RM.Message = "部门名称被占用";
                        }
                        else
                        {
                            collection.CreateOn = DateTime.Now;
                            collection.CreateBy = CurrentHelper.CurrentUser.User.UserName;
                            var res = BusinessContext.tblDepart.Add(collection);
                            RM.IsSuccess = res;
                            RM.IsContinue = IsContinue == "1";
                        }
                    }
                    else
                    {
                        RM.IsSuccess = false;
                        RM.Message = "部门名称不能为空！";
                    }

                }
                catch (Exception ex)
                {
                    RM.Message = ex.Message;
                }
            }
            return Json(RM);
        }

        //
        // GET: /System/Depart/Edit/5

        public ActionResult Edit(int id)
        {
            tblDepart model = BusinessContext.tblDepart.Get(Query<tblDepart>.EQ(t => t.Rid, id));
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // POST: /System/Depart/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, tblDepart collection)
        {
            ReturnMessage RM = new ReturnMessage(false);// new ReturnMessage(false);
            try
            {
                if (!string.IsNullOrEmpty(collection.dept))
                {
                    var q = Query.And(Query<tblDepart>.EQ(t => t.dept, collection.dept),
                        Query<tblDepart>.NE(t => t.Rid, collection.Rid));
                    var query = BusinessContext.tblDepart.GetList(q);
                    //tblDepart old = BusinessContext.tblDepart.Get(Query<tblDepart>.EQ(t => t.Rid, id));
                    if (query.Count > 0)
                    {
                        RM.Message = "部门名称被占用";
                    }
                    else
                    {
                        collection.ModifyOn = DateTime.Now;
                        collection.ModifyBy = CurrentHelper.CurrentUser.User.UserName;
                        RM.IsSuccess = BusinessContext.tblDepart.Update(collection);
                        if (RM.IsSuccess)
                        {
                            //OperateLogHelper.Edit<tblDepart>(collection, old);
                        }
                    }
                }
                else
                {
                    RM.IsSuccess = false;
                    RM.Message = "部门名称不能为空！";
                }
            }
            catch (Exception ex)
            {
                RM.Message = ex.Message;
            }
            return Json(RM);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DataDel()
        {
            ReturnMessage rm = new ReturnMessage();
            try
            {
                string paramData = Request["paramData"];
                if (!string.IsNullOrWhiteSpace(paramData))
                {
                    string[] ridArr = paramData.Split('*');

                    //StringBuilder strSql = new StringBuilder(" delete from tblDepart where Rid in " + DBContext.DataDecision.AssemblyInCondition(RidArr.ToList()));
                    //List<tblDepart> modelList = BusinessContext.tblDepart.GetList(Query<tblDepart>.In(t=>t.Rid,RidArr.Select(int.Parse))).ToList();
                    if (BusinessContext.tblDepart.Delete(ridArr.Select(int.Parse).ToList()))
                    {

                        //OperateLogHelper.Delete<tblDepart>(modelList);
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
