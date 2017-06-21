using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BS.Microservice.Web.Common;
using BS.Common.Model.Mongo.ServiceModels;
using BS.Microservice.Web.Model;
using BS.Client.Common.Model;
using Newtonsoft.Json;
using BS.Common;
using BS.Microservice.Common;
using BS.Microservice.Web.Areas.Service.Models;
using MongoDB.Driver;
using BS.Microservice.Web.Service;
using System.Text.RegularExpressions;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class ManageController : Controller
    {
        //
        // GET: /Service/Manage/

       


        public ActionResult Index()
        {
            ViewBag.BtnList = new List<string>() { "添加", "编辑", "审批", "详细","搜索" };
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ServiceEntity collection,string isContinue="1")
        {
            ReturnMessage rm = new ReturnMessage(false);
            try
            {

                if (string.IsNullOrWhiteSpace(collection.ServiceName))
                {
                    rm.Message = "服务名称不能为空";
                    return Json(rm);
                }

                if (string.IsNullOrWhiteSpace(collection.Host))
                {
                    rm.Message = "部署机器不能为空";
                    return Json(rm);
                }
                if (string.IsNullOrWhiteSpace(collection.Version))
                {
                    rm.Message = "版本号不能为空";
                    return Json(rm);
                }

                if (string.IsNullOrWhiteSpace(collection.SecondaryName))
                {
                    rm.Message = "二级服务名称不能为空";
                    return Json(rm);
                }

                if (BusinessContext.ServiceList.Exists(collection.ServiceName, collection.SecondaryName))
                {
                    rm.Message = "服务已存在";
                    return Json(rm);
                }
                else
                {
                    rm.IsSuccess = BusinessContext.ServiceList.Add(collection);
                    rm.IsContinue = isContinue == "1";
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return Json(rm);
        }

        public JsonResult GetTree()
        {
            var list = BusinessContext.ServiceList.GetTreeModels();
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Edit(int id)
        {

            ServiceEntity model = BusinessContext.ServiceList.GetModel(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrWhiteSpace(model.RegContent))
            {
                try
                {
                    ServiceCfg cfg = JsonConvert.DeserializeObject<ServiceCfg>(model.RegContent);
                    ViewBag.InList = cfg.InAddr;
                    ViewBag.OutList = cfg.OutAddr;
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                }
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(ServiceEntity collection)
        {
            ReturnMessage RM = new ReturnMessage(false);
            try
            {
                List<string> inList = new List<string>();
                List<string> outList = new List<string>();
                string inStr = Request["inAddr"];
                if (!string.IsNullOrWhiteSpace(inStr))
                {
                    inList = inStr.Split(',').ToList();
                }
                string outStr = Request["outAddr"];
                if (!string.IsNullOrWhiteSpace(outStr))
                {
                    outList = outStr.Split(',').ToList();
                }
                ServiceCfg cfg = new ServiceCfg();
                cfg.InAddr = inList;
                cfg.OutAddr = outList;
                cfg.Remarks = collection.Remark;
                ServiceEntity model = BusinessContext.ServiceList.GetModel(Convert.ToInt32(Request["_id"]));
                model.RegContent = JsonConvert.SerializeObject(cfg);

                RM.IsSuccess = BusinessContext.ServiceList.Update(model);
            }
            catch (Exception ex)
            {
                RM.Message = "数据异常,请刷新重试";
                LogManager.Error(ex);
            }

            return Json(RM, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            ServiceEntity model = BusinessContext.ServiceList.GetModel(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewData["serverId"] = model._id;
            ViewData["PrimaryId"] = model.PrimaryId;
            return View(model);
        }

        /// <summary>
        /// 获取用户列表，用于jqGrid展示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDataList(string LoginName = null, int page = 1, int rows = 20, string sidx = "_id", string sord = "asc",string id="",string keyword="")
        {
            int CurrentPageIndex = (page != 0 ? (int)page : 1);
            sidx = string.IsNullOrWhiteSpace(sidx) ? "_id" : sidx;
            List<ServiceEntity> list = BusinessContext.ServiceList.GetModelList(null, sidx, sord, page, rows, id,
                keyword);
            int totalCount = BusinessContext.ServiceList.GetCount(null);
            JqGridData rm = new JqGridData();
            rm.page = CurrentPageIndex;
            rm.rows = list;
            rm.total = totalCount % rows == 0 ? totalCount / rows : totalCount / rows + 1;
            rm.records = totalCount;
            return Json(rm, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegService(int id)
        {
            ReturnMessage RM = new ReturnMessage(false);
            try
            {
                ServiceEntity model = BusinessContext.ServiceList.GetModel(id);
                string serverName = string.Format("{0}/{1}", model.ServiceName, model.SecondaryName);
                RM.IsSuccess = ServerDiscoveryHelper.ServiceRegister(serverName, model.Version, model.RegContent);

            }
            catch (Exception ex)
            {
                RM.Message = "数据异常,请刷新重试";
                LogManager.Error(ex);
            }
            return Json(RM, JsonRequestBehavior.AllowGet);
        }


        static IAppStateService appService = new AppStateServiceFactory().GetInstance();
        static Regex reg_email = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex reg_tel = new Regex(@"^1\d{10}$");

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetPageDataListForContacts(string serviceId, int page = 1, int rows = 50, string sidx = "", string sord = "")
        {

            //分页
            PageInfo pager = new PageInfo();
            pager.CurrentPageIndex = page;
            pager.PageSize = rows;
            pager.Sidx = sidx;
            pager.Sord = sord;
            long total = 0;
            QueryDocument query = new QueryDocument();

            SortByDocument sortBy = new SortByDocument();
            sortBy.Set("LatestTime", -1);
            List<ServiceAlertContactsInfo> list = new List<ServiceAlertContactsInfo>();
            int Id = 0;
            if (!string.IsNullOrWhiteSpace(serviceId) && Int32.TryParse(serviceId, out Id))
            {
                query.Set("ServiceId", Id);
                list = appService.GetPageListServiceAlertContactsInfos(query, sortBy, pager.CurrentPageIndex, pager.PageSize, out total);
            }
            #region 分页处理
            //翻页处理
            SubPageResult<ServiceAlertContactsInfo> result = new SubPageResult<ServiceAlertContactsInfo>();
            result.rows = list;
            result.page = page;
            result.records = (int)total;
            result.total = (result.records + pager.PageSize - 1) / pager.PageSize;
            #endregion
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CreateContact(int id, string pid)
        {
            ViewBag.ServerId = id;
            ViewBag.PrimaryId = pid;
            return View();
        }

        [HttpPost]
        public ActionResult CreateContact(ServiceAlertContactsInfo obj, string IsContinue)
        {
            ReturnMessage RM = new ReturnMessage();

            try
            {
                obj._id = Guid.NewGuid().ToString();
                if (string.IsNullOrWhiteSpace(obj.UserName) || string.IsNullOrWhiteSpace(obj.Tel))
                {
                    RM.Message = "缺少必填字段:姓名、手机号等";
                    return Json(RM);
                }
                if (!reg_tel.IsMatch(obj.Tel))
                {
                    RM.Message = "手机号格式错误";
                    return Json(RM);
                }
                if (!string.IsNullOrWhiteSpace(obj.Email) && !reg_email.IsMatch(obj.Email))
                {
                    RM.Message = "邮箱格式错误";
                    return Json(RM);
                }

                QueryDocument query = new QueryDocument();
                query.Add("UserName", obj.UserName);
                query.Add("ServiceId", obj.ServiceId);
                if (appService.CheckServiceAlertContactsInfos(query) > 0)
                {
                    RM.Message = "该用户已经存在报警清单中";
                }
                else
                {
                    RM.IsSuccess = appService.UpsertServiceAlertContactsInfos(obj);
                    RM.IsContinue = IsContinue == "0" ? false : true; ;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                RM.Message = "保存失败!";
            }

            return Json(RM);
        }

        [HttpGet]
        public ActionResult EditContact(string id)
        {
            ServiceAlertContactsInfo obj = appService.GetServiceAlertContactsInfoById(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult EditContact(string id, ServiceAlertContactsInfo obj)
        {
            ReturnMessage RM = new ReturnMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(obj._id) || string.IsNullOrWhiteSpace(obj.Tel))
                {
                    RM.Message = "缺少必填字段:姓名、手机号等";
                    return Json(RM);
                }
                if (!reg_tel.IsMatch(obj.Tel))
                {
                    RM.Message = "手机号格式错误";
                    return Json(RM);
                }
                if (!string.IsNullOrWhiteSpace(obj.Email) && !reg_email.IsMatch(obj.Email))
                {
                    RM.Message = "邮箱格式错误";
                    return Json(RM);
                }

                IMongoQuery query = Query.And(Query.EQ("UserName", obj.UserName), Query.EQ("ServiceId", obj.ServiceId), Query.Not(Query.EQ("_id", obj._id)));


                RM.IsSuccess = appService.UpsertServiceAlertContactsInfos(obj);

            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                RM.Message = "保存失败!";
            }

            return Json(RM);
        }
        [HttpPost]
        public ActionResult DeleteContact(string paramData)
        {
            ReturnMessage RM = new ReturnMessage();
            List<string> ids = paramData.Split('*').ToList();
            RM.IsSuccess = appService.DelServiceAlertContactsInfos(ids.ToArray());
            if (RM.IsSuccess)
            {
                RM.Message = "删除成功";
            }
            else
            {
                RM.Message = "删除失败";
            }
            return Json(RM);
        }

        public ActionResult GetUserList(string keyword)
        {
            ReturnMessage RM = new ReturnMessage();
            QueryDocument query = new QueryDocument();
            List<ServiceAlertContactsInfo> list = new List<ServiceAlertContactsInfo>();
            list = appService.GetListServiceAlertContactsInfos(query);
            RM.result = list;
            RM.code = 200;
            return Json(RM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SyncAlertData(string id)
        {
            ReturnMessage RM = new ReturnMessage(false);
            try
            {
                int Id = 0;
                if (string.IsNullOrWhiteSpace(id) || !Int32.TryParse(id, out Id))
                {
                    RM.Message = "无效同步服务ID";
                }
                else
                {

                    RM.IsSuccess = appService.SyncAlertData(Id);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                RM.Message = ex.Message;
            }

            return Json(RM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenorCloseAlert(string id)
        {
            ReturnMessage RM = new ReturnMessage(false);
            try
            {
                int Id = 0;
                if (string.IsNullOrWhiteSpace(id) || !Int32.TryParse(id, out Id))
                {
                    RM.Message = "无效服务ID";
                }
                else
                {
                    ServiceEntity model = BusinessContext.ServiceList.GetModel(Id);
                    string msg = "";
                    if (model.IsAlert)
                    {
                        msg = "关闭报警";
                        model.IsAlert = false;
                    }
                    else
                    {
                        msg = "开启报警";
                        model.IsAlert = true;
                    }

                    IMongoQuery query = Query.EQ("ServiceId", Id);
                    BsonDocument bson = new BsonDocument();
                    bson.Set("IsAlert", model.IsAlert);
                    IMongoUpdate update = new UpdateDocument() { { "$set", bson } };
                    DBContext.Mongo.Update("Microservice", "ServiceAlertList", query, update);
                    RM.IsSuccess = BusinessContext.ServiceList.Update(model);
                    if (RM.IsSuccess)
                    {
                        RM.Message = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                RM.Message = ex.Message;
            }

            return Json(RM, JsonRequestBehavior.AllowGet);
        }
    }
}
