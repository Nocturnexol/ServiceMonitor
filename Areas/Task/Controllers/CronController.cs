using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BS.TaskManager.Core.Quartz;

namespace BS.Microservice.Web.Areas.Task.Controllers
{
    public class CronController : Controller
    {
        //
        // GET: /Task/Cron/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CalcRunTime(string CronExpression)
        {
            List<DateTime> list = QuartzHelper.GetNextFireTime(CronExpression, 20);
            List<string> sList = new List<string>();
            list.ForEach(k => sList.Add(k.ToString()));
            return Json(sList, JsonRequestBehavior.AllowGet);
        }
    }
}
