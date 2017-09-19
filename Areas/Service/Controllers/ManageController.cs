using System;
using System.Collections.Generic;
using System.IO;
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
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using CommonHelper = BS.Microservice.Web.Common.CommonHelper;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class ManageController : Controller
    {
        //
        // GET: /Service/Manage/

        private static HSSFWorkbook _hssfworkbook;


        public ActionResult Index(int? type)
        {
            ViewBag.BtnList = CommonHelper.GetBtnAuthorityForPage("服务管理");
            ViewBag.hostList = BusinessContext.ServiceList.GetHostList((ServiceTypeEnum?) type);
            ViewBag.Type = type;
            return View();
        }

        private List<SelectListItem> GetServiceList()
        {
            return BusinessContext.GroupName.GetList().Select(t => new SelectListItem
            {
                Text = t.ServiceNameCN,
                Value = t.ServiceName
            }).ToList();
        }
        public ActionResult Create()
        {
            ViewBag.ServiceList = GetServiceList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(ServiceEntity collection,int? type,string isContinue="1")
        {
            var rm = new ReturnMessage(false);
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
                SetRegContent(collection);
                if (type.HasValue)
                    collection.ServiceType = (ServiceTypeEnum) type.Value;
                rm.IsSuccess = BusinessContext.ServiceList.Add(collection);
                rm.IsContinue = isContinue == "1";
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return Json(rm);
        }

        public JsonResult GetTree(int? type)
        {
            var list = BusinessContext.ServiceList.GetTreeModels((ServiceTypeEnum?) type);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void SetRegContent(ServiceEntity model)
        {
            var inList = new List<string>();
            var outList = new List<string>();
            var inPorts = new List<string>();
            var outPorts = new List<string>();
            var inStr = Request["inAddr"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                inList = inStr.Split(',').ToList();
            }
            var outStr = Request["outAddr"];
            if (!string.IsNullOrWhiteSpace(outStr))
            {
                outList = outStr.Split(',').ToList();
            }
            var inPortStr = Request["inPort"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                inPorts = inPortStr.Split(',').ToList();
            }
            var outPortStr = Request["outPort"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                outPorts = outPortStr.Split(',').ToList();
            }
            var cfg = new ServiceCfg
            {
                InAddr = inList.Zip(inPorts, (t, p) => !string.IsNullOrEmpty(p) ? t + ":" + p : t).ToList(),
                OutAddr = outList.Zip(outPorts, (t, p) => !string.IsNullOrEmpty(p) ? t + ":" + p : t).ToList(),
                Remarks = model.Remark
            };
            model.RegContent = JsonConvert.SerializeObject(cfg);
        }

        public ActionResult Edit(int id)
        {

            var model = BusinessContext.ServiceList.GetModel(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrWhiteSpace(model.RegContent))
            {
                try
                {
                    InitAddr(model);
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                }
            }

            ViewBag.ServiceList = GetServiceList();
            return View(model);
        }

        private void InitAddr(ServiceEntity model)
        {
            var cfg = JsonConvert.DeserializeObject<ServiceCfg>(model.RegContent);
            ViewBag.InList = cfg.InAddr;
            ViewBag.OutList = cfg.OutAddr;
        }

        [HttpPost]
        public ActionResult Edit(ServiceEntity collection)
        {
            var rm = new ReturnMessage(false);
            try
            {
                var model = BusinessContext.ServiceList.GetModel(Convert.ToInt32(Request["_id"]));
                model.ServiceName = collection.ServiceName;
                model.SecondaryName = collection.SecondaryName;
                model.Host = collection.Host;
                model.Version = collection.Version;
                model.Remark = collection.Remark;
                SetRegContent(model);
                rm.IsSuccess = BusinessContext.ServiceList.Update(model);
            }
            catch (Exception ex)
            {
                rm.Message = "数据异常,请刷新重试";
                LogManager.Error(ex);
            }

            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            var model = BusinessContext.ServiceList.GetModel(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewData["serverId"] = model._id;
            ViewData["PrimaryId"] = model.PrimaryId;
            ViewBag.SecondaryId = model.SecondaryId;
            if (!string.IsNullOrWhiteSpace(model.RegContent))
                InitAddr(model);
            return View(model);
        }

        /// <summary>
        /// 获取用户列表，用于jqGrid展示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDataList(int? type,int page = 1, int rows = 20, string sidx = "_id", string sord = "asc",
            string id = "", string keyword = "", string isApproved = "", string host = "")
        {
            sidx = string.IsNullOrWhiteSpace(sidx) ? "_id" : sidx;
            Session["orderBy"] =sidx;
            Session["desc"] = sord;
            Session["id"] = id;
            Session["keyword"] = keyword;
            Session["isApproved"] = isApproved;
            Session["host"] = host;
            Session["type"] = type;
            var currentPageIndex = page != 0 ? page : 1;
            int totalCount;
            var list = BusinessContext.ServiceList.GetModelList((ServiceTypeEnum?)type, sidx, sord, page,
                rows, id,
                keyword, isApproved, host, out totalCount);
            var rm = new JqGridData
            {
                page = currentPageIndex,
                rows = list,
                total = totalCount % rows == 0 ? totalCount / rows : totalCount / rows + 1,
                records = totalCount
            };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegService(int id)
        {
            var rm = new ReturnMessage(false);
            try
            {
                var model = BusinessContext.ServiceList.GetModel(id);
                var serverName = string.Format("{0}/{1}", model.ServiceName, model.SecondaryName);
                model.IsApproved = true;
                rm.IsSuccess = ServerDiscoveryHelper.ServiceRegister(serverName, model.Version, model.RegContent) &&
                               BusinessContext.ServiceList.Update(model);
            }
            catch (Exception ex)
            {
                rm.Message = "数据异常,请刷新重试";
                LogManager.Error(ex);
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ViewResult UploadFiles(int secondaryId)
        {
            ViewBag.SecondaryId = secondaryId;
            return View();
        }
        [HttpPost]
        public JsonResult Upload()
        {
            var rm=new ReturnMessage(false);
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var size = file.ContentLength;
                    var md5 = CommonHelper.GetMd5HashFromFile(file);
                    var model = BusinessContext.Files.Get(Query<FileEntity>.EQ(t => t.Md5, md5));
                    if (model != null)
                    {
                        rm.Message = "文件已存在！";
                        return Json(rm);
                    }
                    var path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var filename = Path.GetFileName(file.FileName);
                    if (filename == null)
                    {
                        rm.Message = "空文件名";
                        return Json(rm);
                    }

                    var extStart = filename.LastIndexOf(".", StringComparison.Ordinal);
                    var ext = filename.Substring(extStart);
                    var newName = string.Format("{0}_{1}{2}", filename.Substring(0, extStart), Guid.NewGuid(), ext);

                    try
                    {
                        var url = Path.Combine(path, filename);
                        var entity = new FileEntity
                        {
                            Author = CurrentHelper.UserName,
                            DateTime = DateTime.Now,
                            FileName = filename,
                            Md5 = md5,
                            Size = size,
                            SecondaryId = int.Parse(Request["SecondaryId"])
                        };
                        if (global::System.IO.File.Exists(url))
                        {
                            url = Path.Combine(path, newName);
                            entity.FileName = newName;
                        }
                        file.SaveAs(url);
                        entity.Url = url;
                        rm.IsSuccess = BusinessContext.Files.Add(entity);
                    }
                    catch (Exception e)
                    {
                        rm.Message = e.Message;
                    }
                }
            }
            return Json(rm);
        }

        public FileResult DownLoad(string fileName)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
            return File(path + fileName, "application/octet-stream");
        }
        public JsonResult GetFileList(int secondaryId,int page = 1, int rows = 20, string sidx = "",
            string sord = "asc")
        {
            if (string.IsNullOrEmpty(sidx))
                sidx = "Rid";
            var pager = new PageInfo
            {
                PageSize = rows,
                CurrentPageIndex = page > 0 ? page : 1
            };

            long totalCount;
            var rm = new JqGridData
            {
                page = pager.CurrentPageIndex,
                rows =
                    BusinessContext.Files.GetList(out totalCount, page, rows,
                        Query<FileEntity>.EQ(t => t.SecondaryId, secondaryId), sidx, sord),
                total =
                    (int)
                    (totalCount % pager.PageSize == 0 ? totalCount / pager.PageSize : totalCount / pager.PageSize + 1),
                records = (int) totalCount
            };
            return Json(rm, JsonRequestBehavior.AllowGet);
        }


        private static readonly IAppStateService AppService = new AppStateServiceFactory().GetInstance();
        private static readonly Regex RegEmail = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegTel = new Regex(@"^1\d{10}$");

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetPageDataListForContacts(string serviceId, int page = 1, int rows = 50, string sidx = "", string sord = "")
        {

            //分页
            var pager = new PageInfo
            {
                CurrentPageIndex = page,
                PageSize = rows,
                Sidx = sidx,
                Sord = sord
            };
            long total = 0;
            var query = new QueryDocument();

            var sortBy = new SortByDocument();
            sortBy.Set("LatestTime", -1);
            var list = new List<ServiceAlertContactsInfo>();
            int id;
            if (!string.IsNullOrWhiteSpace(serviceId) && int.TryParse(serviceId, out id))
            {
                query.Set("ServiceId", id);
                list = AppService.GetPageListServiceAlertContactsInfos(query, sortBy, pager.CurrentPageIndex, pager.PageSize, out total);
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
        public ActionResult CreateContact(ServiceAlertContactsInfo obj, string isContinue)
        {
            var rm = new ReturnMessage();

            try
            {
                obj._id = Guid.NewGuid().ToString();
                if (string.IsNullOrWhiteSpace(obj.UserName) || string.IsNullOrWhiteSpace(obj.Tel))
                {
                    rm.Message = "缺少必填字段:姓名、手机号等";
                    return Json(rm);
                }
                if (!RegTel.IsMatch(obj.Tel))
                {
                    rm.Message = "手机号格式错误";
                    return Json(rm);
                }
                if (!string.IsNullOrWhiteSpace(obj.Email) && !RegEmail.IsMatch(obj.Email))
                {
                    rm.Message = "邮箱格式错误";
                    return Json(rm);
                }

                var query = new QueryDocument {{"UserName", obj.UserName}, {"ServiceId", obj.ServiceId}};
                if (AppService.CheckServiceAlertContactsInfos(query) > 0)
                {
                    rm.Message = "该用户已经存在报警清单中";
                }
                else
                {
                    rm.IsSuccess = AppService.UpsertServiceAlertContactsInfos(obj);
                    rm.IsContinue = isContinue != "0";
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                rm.Message = "保存失败!";
            }

            return Json(rm);
        }

        [HttpGet]
        public ActionResult EditContact(string id)
        {
            var obj = AppService.GetServiceAlertContactsInfoById(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult EditContact(string id, ServiceAlertContactsInfo obj)
        {
            var rm = new ReturnMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(obj._id) || string.IsNullOrWhiteSpace(obj.Tel))
                {
                    rm.Message = "缺少必填字段:姓名、手机号等";
                    return Json(rm);
                }
                if (!RegTel.IsMatch(obj.Tel))
                {
                    rm.Message = "手机号格式错误";
                    return Json(rm);
                }
                if (!string.IsNullOrWhiteSpace(obj.Email) && !RegEmail.IsMatch(obj.Email))
                {
                    rm.Message = "邮箱格式错误";
                    return Json(rm);
                }

                var query = Query.And(Query.EQ("UserName", obj.UserName), Query.EQ("ServiceId", obj.ServiceId), Query.Not(Query.EQ("_id", obj._id)));


                rm.IsSuccess = AppService.UpsertServiceAlertContactsInfos(obj);

            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                rm.Message = "保存失败!";
            }

            return Json(rm);
        }
        [HttpPost]
        public ActionResult DeleteContact(string paramData)
        {
            var rm = new ReturnMessage();
            var ids = paramData.Split('*').ToList();
            rm.IsSuccess = AppService.DelServiceAlertContactsInfos(ids.ToArray());
            rm.Message = rm.IsSuccess ? "删除成功" : "删除失败";
            return Json(rm);
        }
        [HttpPost]
        public ActionResult DeleteFiles(string paramData)
        {
            var rm = new ReturnMessage();
            var ids = paramData.Split(new[] {'*'}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            var files = BusinessContext.Files.GetList(Query<FileEntity>.In(t => t.Rid, ids));
            foreach (var file in files)
            {
                if (global::System.IO.File.Exists(file.Url))
                {
                    global::System.IO.File.Delete(file.Url);
                }
            }
            rm.IsSuccess = BusinessContext.Files.Delete(ids);
            rm.Message = rm.IsSuccess ? "删除成功" : "删除失败";
            return Json(rm);
        }
        public ActionResult GetUserList(string keyword)
        {
            var rm = new ReturnMessage();
            var query = new QueryDocument();
            var list = AppService.GetListServiceAlertContactsInfos(query);
            rm.result = list;
            rm.code = 200;
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SyncAlertData(string id)
        {
            var rm = new ReturnMessage(false);
            try
            {
                int Id = 0;
                if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out Id))
                {
                    rm.Message = "无效同步服务ID";
                }
                else
                {

                    rm.IsSuccess = AppService.SyncAlertData(Id);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                rm.Message = ex.Message;
            }

            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenorCloseAlert(string id)
        {
            var rm = new ReturnMessage(false);
            try
            {
                var Id = 0;
                if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out Id))
                {
                    rm.Message = "无效服务ID";
                }
                else
                {
                    var model = BusinessContext.ServiceList.GetModel(Id);
                    string msg;
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

                    var query = Query.EQ("ServiceId", Id);
                    var bson = new BsonDocument();
                    bson.Set("IsAlert", model.IsAlert);
                    IMongoUpdate update = new UpdateDocument { { "$set", bson } };
                    DBContext.Mongo.Update("Microservice", "ServiceAlertList", query, update);
                    rm.IsSuccess = BusinessContext.ServiceList.Update(model);
                    if (rm.IsSuccess)
                    {
                        rm.Message = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                rm.Message = ex.Message;
            }

            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string filename)
        {
            filename = filename.Split('?')[0];
            var rm = new ReturnMessage(false);
            try
            {
                string orderBy = Session["orderBy"] as string ?? "_id";
                string desc = Session["desc"] as string ?? "asc";
                string id = Session["id"] as string;
                string keyword = Session["keyword"] as string;
                string isApproved = Session["isApproved"] as string;
                string host = Session["host"] as string;
                var type = Session["type"] as int?;
                int count;
                var list = BusinessContext.ServiceList.GetModelList((ServiceTypeEnum?)type, orderBy, desc, 1, int.MaxValue,id, keyword, isApproved, host,
                    out count);

                if (list == null || !list.Any())
                {
                    rm.Message = "没有数据！";
                    return Json(rm, JsonRequestBehavior.AllowGet);
                }
                string title = filename;
                InitializeWorkbook(title);
                NPOI.SS.UserModel.ISheet sheet1 = _hssfworkbook.GetSheetAt(0);
                //sheet1.GetRow(1).GetCell(5).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                //下移
                int rows = 2;
                int num = 1;
                foreach (var item in list)
                {
                    sheet1.CreateRow(rows);
                    sheet1.GetRow(rows).CreateCell(0);
                    sheet1.GetRow(rows).GetCell(0).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(0).SetCellValue(num);
                    num++;

                    sheet1.GetRow(rows).CreateCell(1);
                    sheet1.GetRow(rows).GetCell(1).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(1).SetCellValue(item.PrimaryId);

                    sheet1.GetRow(rows).CreateCell(2);
                    sheet1.GetRow(rows).GetCell(2).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(2).SetCellValue(item.SecondaryId);

                    ServiceCfg cfg = new ServiceCfg();
                    if (!string.IsNullOrWhiteSpace(item.RegContent))
                        cfg = JsonConvert.DeserializeObject<ServiceCfg>(item.RegContent);
                    var inList = cfg.InAddr ?? new List<string>();
                    var outList = cfg.OutAddr ?? new List<string>();

                    var inFlag = inList.FirstOrDefault(t => t.Contains(item.Host)) != null;
                    var inAddr = inFlag
                        ? inList.FirstOrDefault(t => t.Contains(item.Host))
                            .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries)
                        : new string[0];

                    sheet1.GetRow(rows).CreateCell(3);
                    sheet1.GetRow(rows).GetCell(3).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows)
                        .GetCell(3)
                        .SetCellValue(item.Host);

                    sheet1.GetRow(rows).CreateCell(4);
                    sheet1.GetRow(rows).GetCell(4).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(4).SetCellValue(inFlag ? inAddr.Length > 1 ? inAddr[1] : "" : "");

                    var outAddr = new List<string>();
                    var outPort = new List<string>();
                    if (outList.Any())
                    {
                        var @out = outList.Select(t =>
                        {
                            var arr = t.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                            return new {Addr = arr[0], Port = arr.Length > 1 ? arr[1] : ""};
                        }).ToList();
                        outAddr=@out.Select(t=>t.Addr).ToList();
                        outPort = @out.Select(t => t.Port).ToList();
                    }

                    sheet1.GetRow(rows).CreateCell(5);
                    sheet1.GetRow(rows).GetCell(5).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(5).SetCellValue(string.Join(",", outAddr));


                    sheet1.GetRow(rows).CreateCell(6);
                    sheet1.GetRow(rows).GetCell(6).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(6).SetCellValue(string.Join(",", outPort));


                    sheet1.GetRow(rows).CreateCell(7);
                    sheet1.GetRow(rows).GetCell(7).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(7).SetCellValue(item.ServiceName);

                    sheet1.GetRow(rows).CreateCell(8);
                    sheet1.GetRow(rows).GetCell(8).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(8).SetCellValue(item.SecondaryName);

                    sheet1.GetRow(rows).CreateCell(9);
                    sheet1.GetRow(rows).GetCell(9).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(9).SetCellValue(item.Version);

                    sheet1.GetRow(rows).CreateCell(10);
                    sheet1.GetRow(rows).GetCell(10).CellStyle = sheet1.GetRow(1).GetCell(5).CellStyle;
                    sheet1.GetRow(rows).GetCell(10).SetCellValue(item.Remark);

                    rows++;
                }

                sheet1.ForceFormulaRecalculation = true;
                var pathLoad = WriteToFile(filename);
                //弹出下载框
                rm.IsSuccess = true;
                rm.Text = HttpUtility.UrlEncode(pathLoad);
            }
            catch (Exception ex)
            {
                rm.IsSuccess = false;
                rm.Message = ex.Message;
            }
            return Json(rm, JsonRequestBehavior.AllowGet);
        }

        private static void InitializeWorkbook(string excelName)
        {
            //E:\visual studio 2010\Projects\文本解析\AjaxExportExcel\AjaxExportExcel
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            //basePath = Environment.CurrentDirectory;
            FileStream file = new FileStream(basePath + "Excel\\" + excelName + ".xls", FileMode.Open, FileAccess.Read);
            _hssfworkbook = new HSSFWorkbook(file);
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            _hssfworkbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            _hssfworkbook.SummaryInformation = si;
        }
        private static string WriteToFile(string filename)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            //创建临时文件夹 判断是否存在，不存在文件夹就创建出来
            string tmpPath = basePath + "TempData";
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }

            string path = string.Format("TempData\\{0}", filename);
            string dPath = path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            string tPath = basePath + dPath;
            FileStream file = new FileStream(tPath, FileMode.Create);
            _hssfworkbook.Write(file);
            file.Close();
            return tPath;
        }

    }
}
