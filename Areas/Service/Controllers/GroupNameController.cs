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
using ServiceEntity = BS.Microservice.Web.Model.ServiceEntity;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class GroupNameController : Controller
    {
        private readonly MongoBll<GroupName> _bll = new MongoBll<GroupName>();
        public ActionResult Index()
        {
            var btnList = CommonHelper.GetBtnAuthorityForPage("服务名称管理");
            ViewBag.BtnList = btnList;
            var maxPrimaryId = (int) (_bll.Max(t => t._id) ?? 0);
            //var list = DBContext.Mongo.Find<ServiceEntity>(DBContext.DbName, "ServiceList",
            //    , new List<string> { "ServiceName", "PrimaryId" });
            var list = BusinessContext.ServiceList.GetList(Query<ServiceEntity>.GT(t => t.PrimaryId, maxPrimaryId));
            var gList = list.GroupBy(p => new { p.PrimaryId, p.ServiceName }).
                Select(p => new GroupName
                {
                    _id = p.Key.PrimaryId,
                    ServiceName = p.Key.ServiceName,
                    ServiceNameCN = p.Key.ServiceName,
                    CreateDateTime = DateTime.Now
                }).ToList();
            if (gList.Count > 0)
            {
                _bll.Add(gList);
            }
            return View();
        }
        public ActionResult GetGroupNameList(int page = 1, int rows = 20, GroupName search = null, string sidx = null, string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "_id";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            List<IMongoQuery> queryList = null;
            if (search != null)
            {
                queryList = new List<IMongoQuery>();
                if (search._id>0)
                {
                    queryList.Add(Query<GroupName>.EQ(t => t._id, search._id));
                }
                if (!string.IsNullOrWhiteSpace(search.ServiceName))
                {
                    var reg = new BsonRegularExpression(new Regex(search.ServiceName, RegexOptions.IgnoreCase));
                    queryList.Add(Query.Or(Query<GroupName>.Matches(t => t.ServiceName, reg),
                        Query<GroupName>.Matches(t => t.ServiceNameCN, reg)));
                }
            }
            var where = queryList != null && queryList.Any() ? Query.And(queryList) : null;

            long totalCount;
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

        [HttpPost]
        public ActionResult Create(GroupName collection, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(collection.ServiceName))
                    {
                        if (!CheckRepeat(collection))
                        {
                            rm.IsSuccess = false;
                            rm.Message = "已有相同记录存在！";
                        }
                        else
                        {
                            var max = (int) (_bll.Max(t => t._id) ?? 0);
                            collection._id = max + 1;
                            collection.CreateDateTime = DateTime.Now;
                            collection.CreateBy = CurrentHelper.CurrentUser.User.UserName;
                            rm.IsSuccess = _bll.Add(collection);
                            if (rm.IsSuccess)
                            {
                                //OperateLogHelper.Create(collection);
                                rm.IsContinue = isContinue == "1";
                            }
                        }
                    }
                    else
                    {
                        rm.IsSuccess = false;
                        rm.Message = "名称不能为空！";
                    }

                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            return Json(rm);
        }
        public ActionResult Delete()
        {
            var rm = new ReturnMessage();
            try
            {
                string paramData = Request.Form["paramData"];
                if (!string.IsNullOrWhiteSpace(paramData))
                {
                    string[] ids = paramData.Split('*');
                    var rIds = ids.Select(int.Parse).ToList();
                    if (_bll.Delete(rIds))
                    {
                        //OperateLogHelper.Delete(modelList);
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

        public ActionResult Edit(int id)
        {
            var model = _bll.Get(Query.EQ("Rid", id));
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.BasicTypeList = CommonHelper.BasicTypeList;
            return View(model);

        }
        [HttpPost]
        public ActionResult Edit(GroupName collection)
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!CheckRepeat(collection))
                    {
                        rm.IsSuccess = false;
                        rm.Message = "已有相同记录存在！";
                    }
                    else
                    {
                        collection.ModifyOn = DateTime.Now;
                        collection.ModifyBy = CurrentHelper.CurrentUser.User.UserName;
                        rm.IsSuccess = _bll.Update(collection);
                    }
                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            return Json(rm);
        }

        private bool CheckRepeat(GroupName model)
        {
            var query = Query.Or(Query<GroupName>.EQ(t => t._id, model._id),
                Query<GroupName>.EQ(t => t.ServiceName, model.ServiceName));
            var modelDb = _bll.Get(query);
            if (modelDb == null) return true;
            return model.Rid == modelDb.Rid;
        }
    }
}
