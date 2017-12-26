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
    public class VirtualDeviceController : Controller
    {
        private readonly MongoBll<VirtualDevice> _bll = new MongoBll<VirtualDevice>();
        public ActionResult Index(bool? isDeployment)
        {
            var btnList = CommonHelper.GetBtnAuthorityForPage("虚拟设备");
            if (isDeployment.HasValue)
                ViewBag.IsDeployment = isDeployment;
            else
                ViewBag.BtnList = btnList;
            return View();
        }
        public ActionResult GetVirtualDeviceList(int page = 1, int rows = 20, VirtualDevice search = null, string sidx = null, string sord = "asc")
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
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.MachineName,
                        new BsonRegularExpression(new Regex(search.MachineName, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.ModelNum))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.ModelNum,
                        new BsonRegularExpression(new Regex(search.ModelNum, RegexOptions.IgnoreCase))));
                }
                if (search.HostDevice.HasValue)
                {
                    queryList.Add(Query<VirtualDevice>.EQ(t => t.HostDevice, search.HostDevice));
                }
                if (!string.IsNullOrWhiteSpace(search.PublicIP))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.PublicIP, search.PublicIP));
                }
                if (!string.IsNullOrWhiteSpace(search.IntranetIP))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.IntranetIP, search.IntranetIP));
                }
                if (!string.IsNullOrWhiteSpace(search.DomainIP))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.DomainIP, search.DomainIP));
                }
                if (!string.IsNullOrWhiteSpace(search.Cpu))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.Cpu,
                        new BsonRegularExpression(new Regex(search.Cpu, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.Memory))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.Memory,
                        new BsonRegularExpression(new Regex(search.Memory, RegexOptions.IgnoreCase))));
                }
                if (!string.IsNullOrWhiteSpace(search.Storage))
                {
                    queryList.Add(Query<VirtualDevice>.Matches(t => t.Storage,
                        new BsonRegularExpression(new Regex(search.Storage, RegexOptions.IgnoreCase))));
                }
                if (search.Date.HasValue)
                {
                    queryList.Add(Query<VirtualDevice>.EQ(t => t.Date, search.Date));
                }
                if (search.StartDate.HasValue)
                {
                    queryList.Add(Query<VirtualDevice>.EQ(t => t.StartDate, search.StartDate));
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
        public string GetVirtualDeviceName(int rId)
        {
            var model = _bll.Get(Query<VirtualDevice>.EQ(t => t.Rid, rId));
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
        public ActionResult Create(VirtualDevice collection, string isContinue = "0")
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
                            collection.CreateOn = DateTime.Now;
                            collection.CreateBy = CurrentHelper.CurrentUser.User.UserName;
                            var res = _bll.Add(collection);
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
            ViewBag.HostDeviceName = model.HostDevice.HasValue
                ? DependencyResolver.Current.GetService<PhysicalDeviceController>()
                    .GetHostDeviceName(model.HostDevice.Value)
                : "";
            return View(model);

        }

        [HttpPost]
        public ActionResult Edit(VirtualDevice collection)
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
            else
            {
                rm.IsSuccess = false;
                rm.Message = "数据格式不正确";
            }
            return Json(rm);
        }

        private bool CheckRepeat(VirtualDevice model)
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
