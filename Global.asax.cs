using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BS.Common;
using BS.Common.Model.Mongo.ServiceModels;
using BS.Microservice.Common;
using BS.Microservice.Web.BLL;
using BS.Microservice.Web.Common;
using BS.Microservice.Web.Model;
using MongoDB.Driver.Builders;
using CommonHelper = BS.Microservice.Web.Common.CommonHelper;

namespace BS.Microservice.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            CommonHelper.InitBasicType();
            AreaRegistration.RegisterAllAreas();
            ConfigHelper.InitConfig<AppSetting>();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
         var t=   CurrentHelper.SerCfg;
            BundleTable.EnableOptimizations = true;
            //InitGroupNameCol();
        }
        //public void InitGroupNameCol()
        //{
        //    try
        //    {
               

        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Error(ex);
        //    }
        //}
    }
}