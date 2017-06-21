using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using BS.Microservice.Web.Model;
using BS.Microservice.Web.Areas.Service.Models;
using BS.Microservice.Web.Service;
using System.Text.RegularExpressions;
using BS.Client.Common;

namespace BS.Microservice.Web.Areas.Service.Controllers
{
    public class MonitorController : Controller
    {
        //
        // GET: /Service/Monitor/
       
        public ActionResult Index()
        {
            return View();
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
        public ActionResult GetPageDataListForContacts(int page = 1, int rows = 50, string sidx = "", string sord = "")
        {
            string Manager = Request["Manager"];
            //分页
            PageInfo pager = new PageInfo();
            pager.CurrentPageIndex = page;
            pager.PageSize = rows;
            pager.Sidx = sidx;
            pager.Sord = sord;
            long total = 0;
            QueryDocument query = new QueryDocument();
            if (!string.IsNullOrWhiteSpace(Manager))
            {
                query.Set("_id", new MongoDB.Bson.BsonRegularExpression(Manager));
            }
            SortByDocument sortBy = new SortByDocument();
            sortBy.Set("LatestTime", -1);
            var list = appService.GetPageListAppContacts(query, sortBy, pager.CurrentPageIndex, pager.PageSize, out total);

            #region 分页处理
            //翻页处理
            SubPageResult<AppContactsInfo> result = new SubPageResult<AppContactsInfo>();
            result.rows = list;
            result.page = page;
            result.records = (int)total;
            result.total = (result.records + pager.PageSize - 1) / pager.PageSize;
            #endregion
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(AppContactsInfo obj, string IsContinue)
        {
            ReturnMessage RM = new ReturnMessage();
            if (ModelState.IsValid)
            {
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
                    RM.IsSuccess = appService.UpsertAppContacts(obj);
                    RM.IsContinue = IsContinue == "0" ? false : true; ;
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    RM.Message = "保存失败!";
                }
            }
            return Json(RM);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            AppContactsInfo obj = appService.GetAppContactsById(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Edit(string id, AppContactsInfo obj)
        {
            ReturnMessage RM = new ReturnMessage();
            if (ModelState.IsValid)
            {
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
                    RM.IsSuccess = appService.UpsertAppContacts(obj);
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    RM.Message = "保存失败!";
                }
            }
            return Json(RM);
        }
        [HttpPost]
        public ActionResult Delete(string paramData)
        {
            ReturnMessage RM = new ReturnMessage();
            List<string> ids = paramData.Split('*').ToList();
            RM.IsSuccess = appService.DelAppContacts(ids.ToArray());
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
    }
}
