using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BS.Microservice.Web.BLL;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class PhysicalDeviceController : Controller
    {
        private readonly MongoBll<PhysicalDevice> _bll = new MongoBll<PhysicalDevice>();
        public ActionResult Index(bool? isHost)
        {
            var btnList = CommonHelper.GetBtnAuthorityForPage("物理设备");
            ViewBag.TypeList = CommonHelper.GetTypeSelectList("设备类型");
            ViewBag.OwnerList = CommonHelper.GetTypeSelectList("业主方");
            if (isHost != null)
                ViewBag.IsHost = isHost;
            else
                ViewBag.BtnList = btnList;
            return View();
        }
        public ActionResult GetPhysicalDeviceList(int page = 1, int rows = 20, PhysicalDevice search = null, string sidx = null, string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "MachineName";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };
            List<IMongoQuery> queryList = null;
            if (search != null)
            {
                queryList = new List<IMongoQuery>();
                if (!string.IsNullOrWhiteSpace(search.MachineName))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.MachineName,
                        new BsonRegularExpression(new Regex(search.MachineName, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.ModelNum))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.ModelNum,
                        new BsonRegularExpression(new Regex(search.ModelNum, RegexOptions.IgnoreCase))));
                }
                if (search.DeviceType.HasValue)
                {
                    queryList.Add(Query<PhysicalDevice>.EQ(t => t.DeviceType, search.DeviceType.Value));
                }
                if (search.Owner.HasValue)
                {
                    queryList.Add(Query<PhysicalDevice>.EQ(t => t.Owner, search.Owner.Value));
                }
                if (!string.IsNullOrWhiteSpace(search.PublicIP))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.PublicIP, search.PublicIP));
                }
                if (!string.IsNullOrWhiteSpace(search.IntranetIP))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.IntranetIP, search.IntranetIP));
                }
                if (!string.IsNullOrWhiteSpace(search.DomainIP))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.DomainIP, search.DomainIP));
                }
                if (!string.IsNullOrWhiteSpace(search.ManagementIP))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.ManagementIP, search.ManagementIP));
                }
                if (!string.IsNullOrWhiteSpace(search.Cpu))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.Cpu,
                        new BsonRegularExpression(new Regex(search.Cpu, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.Memory))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.Memory,
                        new BsonRegularExpression(new Regex(search.Memory, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.Storage))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.Storage,
                        new BsonRegularExpression(new Regex(search.Storage, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.Locale))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.Locale,
                        new BsonRegularExpression(new Regex(search.Locale, RegexOptions.IgnoreCase))));
                }
                if (search.Date.HasValue)
                {
                    queryList.Add(Query<PhysicalDevice>.EQ(t => t.Date, search.Date));
                }
                if (search.PurchaseDate.HasValue)
                {
                    queryList.Add(Query<PhysicalDevice>.EQ(t => t.PurchaseDate, search.PurchaseDate));
                }
                if (search.WarrantyExpiry.HasValue)
                {
                    queryList.Add(Query<PhysicalDevice>.EQ(t => t.WarrantyExpiry, search.WarrantyExpiry));
                }
                if (!string.IsNullOrWhiteSpace(search.NetTpParam))
                {
                    queryList.Add(Query<PhysicalDevice>.Matches(t => t.NetTpParam,
                        new BsonRegularExpression(new Regex(search.NetTpParam, RegexOptions.IgnoreCase))));
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

        public string GetHostDeviceName(int rId)
        {
            var model = _bll.Get(Query<PhysicalDevice>.EQ(t => t.Rid, rId));
            if (model == null)
                throw new HttpException(404, "记录未找到");
            return model.MachineName;
        }
        public ActionResult Create()
        {
            ViewBag.TypeList = CommonHelper.GetTypeSelectList("设备类型");
            ViewBag.OwnerList = CommonHelper.GetTypeSelectList("业主方");
            return View();
        }

        [HttpPost]
        public ActionResult Create(PhysicalDevice collection, string isContinue = "0")
        {
            var rm = new ReturnMessage(false);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(collection.ModelNum))
                    {
                        if (!CheckRepeat(collection))
                        {
                            rm.IsSuccess = false;
                            rm.Message = "已有相同记录存在！";
                        }
                        else
                        {
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
                        rm.Message = "型号不能为空！";
                    }

                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            else
            {
                rm.IsSuccess = false;
                rm.Message = "数据格式不正确";
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
            ViewBag.TypeList = CommonHelper.GetTypeSelectList("设备类型");
            ViewBag.OwnerList = CommonHelper.GetTypeSelectList("业主方");
            return View(model);

        }
        [HttpPost]
        public ActionResult Edit(PhysicalDevice collection)
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
                        rm.IsSuccess = _bll.Update(collection);
                    }
                }
                catch (Exception ex)
                {
                    rm.Message = ex.Message;
                }
            }
            else
            {
                rm.IsSuccess = false;
                rm.Message = "数据格式不正确";
            }
            return Json(rm);
        }

        private bool CheckRepeat(PhysicalDevice model)
        {
            var query = Query.And(Query.EQ("PublicIP", model.PublicIP),
                Query.EQ("IntranetIP", model.IntranetIP),
                Query.EQ("MachineName", model.MachineName));
            var modelDb = _bll.Get(query);
            if (modelDb == null) return true;
            return model.Rid == modelDb.Rid;
        }

    }
}
