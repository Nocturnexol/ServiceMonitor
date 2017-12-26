using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BS.Microservice.Web.BLL;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class DeviceTypeController : Controller
    {
        private readonly MongoBll<BasicType> _bll = new MongoBll<BasicType>();
        public ActionResult Index()
        {
            List<string> btnList = CommonHelper.GetBtnAuthorityForPage("基础数据");
            ViewBag.BtnList = btnList;
            ViewBag.BasicTypeList = CommonHelper.BasicTypeList;
            return View();
        }
        public ActionResult GetDeviceTypeList(int page = 1, int rows = 20, BasicType search = null, string sidx = null, string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "TypeId";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            List<IMongoQuery> queryList = null;
            if (search != null)
            {
                queryList = new List<IMongoQuery>();
                if (search.TypeId.HasValue)
                {
                    queryList.Add(Query<BasicType>.EQ(t => t.TypeId, search.TypeId));
                }
                if (search.Num.HasValue)
                {
                    queryList.Add(Query<BasicType>.EQ(t => t.Num, search.Num));
                }
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    queryList.Add(Query<BasicType>.EQ(t => t.Name, search.Name));
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
            ViewBag.BasicTypeList = CommonHelper.BasicTypeList;
            return View();
        }

        [HttpPost]
        public ActionResult Create(BasicType collection, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(collection.Name))
                    {
                        if (!CheckRepeat(collection))
                        {
                            rm.IsSuccess = false;
                            rm.Message = "已有相同记录存在！";
                        }
                        else
                        {
                            collection.CreateOn = DateTime.Now;
                            collection.CreateBy = CurrentHelper.CurrentUser.User.UserName;
                            bool res = _bll.Add(collection);
                            rm.IsSuccess = res;
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
        public ActionResult Edit(BasicType collection)
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

        private bool CheckRepeat(BasicType model)
        {
            var query = Query.And(Query.EQ("Num", model.Num), Query.EQ("TypeId", model.TypeId));
            var modelDb = _bll.Get(query);
            if (modelDb == null) return true;
            return model.Rid == modelDb.Rid;
        }
    }
}
