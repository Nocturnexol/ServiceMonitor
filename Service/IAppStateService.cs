using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using BS.Microservice.Web.Areas.Service.Models;

namespace BS.Microservice.Web.Service
{
    /// <summary>
    /// 应用程序状态服务
    /// </summary>
    public interface IAppStateService : IBaseService
    {
        /// <summary>
        /// 根据id获取App联系人信息
        /// </summary>
        /// <returns></returns>
        AppContactsInfo GetAppContactsById(string id);
        /// <summary>
        /// 分页查询App联系人信息
        /// </summary>
        /// <returns></returns>
        List<AppContactsInfo> GetPageListAppContacts(IMongoQuery query, IMongoSortBy sortBy, int pageNo, int pageSize, out long total);
        /// <summary>
        /// 添加或新增App联系人信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool UpsertAppContacts(AppContactsInfo obj);

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="ids">联系人姓名列表</param>
        /// <returns></returns>
        bool DelAppContacts(string[] ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        long CheckServiceAlertContactsInfos(IMongoQuery query);


        /// <summary>
        /// 根据id获取App联系人信息
        /// </summary>
        /// <returns></returns>
        ServiceAlertContactsInfo GetServiceAlertContactsInfoById(string id);
        /// <summary>
        /// 分页查询App联系人信息
        /// </summary>
        /// <returns></returns>
        List<ServiceAlertContactsInfo> GetPageListServiceAlertContactsInfos(IMongoQuery query, IMongoSortBy sortBy, int pageNo, int pageSize, out long total);
        /// <summary>
        /// 添加或新增App联系人信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool UpsertServiceAlertContactsInfos(ServiceAlertContactsInfo obj);

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="ids">联系人姓名列表</param>
        /// <returns></returns>
        bool DelServiceAlertContactsInfos(string[] ids);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ServiceAlertContactsInfo> GetListServiceAlertContactsInfos(IMongoQuery query);


        bool SyncAlertData(int id);
    }
}