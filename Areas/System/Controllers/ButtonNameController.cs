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
    public class ButtonNameController : Controller
    {
        //
        // GET: /System/ButtonName/

        public ActionResult Index()
        {
            //按钮权限
            List<string> btnList = CommonHelper.GetBtnAuthorityForPage("按钮名称管理");
            ViewBag.BtnList = btnList;
            return View();
        }


        public ActionResult GetButtonNameDataList(int page = 1, int rows = 20, string keyword = null, string sidx = "ButtonName", string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "Rid";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            List<IMongoQuery> queryList = null;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryList = new List<IMongoQuery>
                {
                    Query<tblButtonName>.Matches(t => t.ButtonName,
                        new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase)))
                };
            }
            var where = queryList != null && queryList.Any() ? Query.And(queryList) : null;

            long totalCount;
            var rm = new JqGridData
            {
                page = pager.CurrentPageIndex,
                rows = BusinessContext.tblButtonName.GetList(out totalCount, page, rows, where, sidx, sord),
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
        public ActionResult Create(tblButtonName collection, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(collection.ButtonName))
                    {
                        var query =
                            BusinessContext.tblButtonName.GetList(Query<tblButtonName>.EQ(t => t.ButtonName,
                                collection.ButtonName));
                        if (query.Count > 0)
                        {
                            rm.Message = "按钮名称已被占用";
                        }
                        else
                        {
                            var res = BusinessContext.tblButtonName.Add(collection);
                            rm.IsSuccess = res;
                            if (rm.IsSuccess)
                            {
                                rm.IsContinue = isContinue == "1";
                            }
                        }
                    }
                    else
                    {
                        rm.IsSuccess = false;
                        rm.Message = "按钮名称不能为空！";
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

            tblButtonName dpt = BusinessContext.tblButtonName.Get(Query<tblButtonName>.EQ(t => t.Rid, id));
            if (dpt == null)
            {
                return HttpNotFound();
            }
            return View(dpt);

        }
        [HttpPost]
        public ActionResult Edit(tblButtonName collection)
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    var q = Query.And(Query<tblButtonName>.EQ(t => t.ButtonName, collection.ButtonName),
                        Query<tblButtonName>.NE(t => t.Rid, collection.Rid));
                    var query = BusinessContext.tblButtonName.GetList(q);
                    if (query.Count > 0)
                    {
                        rm.Message = "按钮名称已被占用";
                    }
                    else
                    {
                        rm.IsSuccess = BusinessContext.tblButtonName.Update(collection);
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
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DataDel()
        {
            var rm = new ReturnMessage();
            try
            {
                string paramData = Request.Form["paramData"];
                if (!string.IsNullOrWhiteSpace(paramData))
                {
                    var ridArr = paramData.Split('*').Select(int.Parse).ToList();
                    var btnNameIds =
                        BusinessContext.tblGroupButton.GetList(Query<tblGroupButton>.In(t => t.ButtonNameId,
                            BusinessContext.tblButtonName.GetList(Query<tblButtonName>.In(t => t.Rid, ridArr))
                                .Select(x => (int?) x.Rid)));
                    //StringBuilder strSql = new StringBuilder(" select ButtonNameId from tblGroupButton where ButtonNameId in(select Rid from tblButtonName where Rid in " + DBContext.DataDecision.AssemblyInCondition(RidArr.ToList()) + ")");
                    //DataTable dt = DBContext.DataDecision.GetDataTable(strSql.ToString());

                    //strSql = new StringBuilder(" select ButtonNameId from tblDetailButton where ButtonNameId in(select Rid from tblButtonName where Rid in " + DBContext.DataDecision.AssemblyInCondition(RidArr.ToList()) + ")");
                    //DataTable Detail_dt = DBContext.DataDecision.GetDataTable(strSql.ToString());

                    ////List<tblButtonName> modelList = BusinessContext.tblButtonName.GetModelList("Rid in " + DBContext.AssemblyInCondition(RidArr.ToList())).ToList();
                    if (btnNameIds.Any())
                    {
                        rm.IsSuccess = false;
                        rm.Message = "当前按钮已被使用！";
                    }
                    else
                    {
                        //strSql = new StringBuilder(" delete from tblButtonName where Rid in " + DBContext.DataDecision.AssemblyInCondition(RidArr.ToList()));
                        //DataTable dt = DBContext.BusDataSys.GetDataTable(strSql.ToString());
                        if (BusinessContext.tblButtonName.Delete(ridArr))
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
