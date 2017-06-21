using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Microservice.Web.Areas.Service.Models;
using MongoDB.Driver;
using BS.Microservice.Web.Common;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using BS.Common.Model.Mongo.ServiceModels;
using BS.Common;
namespace BS.Microservice.Web.Service
{
    #region 工厂定义
    /// <summary>
    /// App状态数据服务工厂
    /// hejh
    /// 2017-3-24
    /// </summary>
    public class AppStateServiceFactory : BaseFactory<IAppStateService>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppStateServiceFactory()
            : base(typeof(AppStateService))
        {
        }
    }
    #endregion

    #region 实现类
    /// <summary>
    /// GPS位置数据服务
    /// hejh
    /// 2016/8/11
    /// </summary>
    internal class AppStateService : IAppStateService
    {
        // MongoDB库名BSTAM
        public readonly string DBName_BSTAM = DBContext.DbName;
        // MongoDB表名AppState
        public readonly string TableName_ServiceAlert = "ServiceAlertList";
        // MongoDB表名AppContacts
        public readonly string TableName_AppContacts = "App_Contacts";


        public readonly string TableName_ServiceList = "ServiceList";
        #region 联系人
        /// <summary>
        /// 根据id获取App联系人信息
        /// </summary>
        /// <returns></returns>
        public AppContactsInfo GetAppContactsById(string id)
        {
            AppContactsInfo obj = null;
            try
            {
                IMongoQuery query = Query.EQ("_id", id);
                obj = DBContext.Mongo.FindOne<AppContactsInfo>(DBName_BSTAM, TableName_AppContacts, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return obj;
        }
        /// <summary>
        /// 分页查询App联系人信息
        /// </summary>
        /// <returns></returns>
        public List<AppContactsInfo> GetPageListAppContacts(IMongoQuery query, IMongoSortBy sortBy, int pageNo, int pageSize, out long total)
        {
            total = 0;
            List<AppContactsInfo> list = new List<AppContactsInfo>();
            try
            {
                total = DBContext.Mongo.Count(DBName_BSTAM, TableName_AppContacts, query);
                list = DBContext.Mongo.GetPageList<AppContactsInfo>(DBName_BSTAM, TableName_AppContacts, query, pageNo, pageSize, sortBy);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return list;
        }


        /// <summary>
        /// 添加或新增App联系人信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpsertAppContacts(AppContactsInfo obj)
        {
            bool ret = false;
            if (obj == null || string.IsNullOrWhiteSpace(obj._id))
            {
                return ret;
            }
            try
            {
                ret = DBContext.Mongo.Upsert<AppContactsInfo>(DBName_BSTAM, TableName_AppContacts, obj);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return ret;
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="ids">联系人姓名列表</param>
        /// <returns></returns>
        public bool DelAppContacts(string[] ids)
        {
            bool ret = false;
            if (ids == null || ids.Length == 0)
            {
                return ret;
            }
            try
            {
                IMongoQuery query = Query.In("_id", ids.Select(p => new BsonString(p)));
                ret = DBContext.Mongo.Remove(DBName_BSTAM, TableName_AppContacts, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return ret;
        }
        #endregion

        #region 服务报警联系人
        /// <summary>
        /// 根据id获取App联系人信息
        /// </summary>
        /// <returns></returns>
        public ServiceAlertContactsInfo GetServiceAlertContactsInfoById(string id)
        {
            ServiceAlertContactsInfo obj = null;
            try
            {
                IMongoQuery query = Query.EQ("_id", id);
                obj = DBContext.Mongo.FindOne<ServiceAlertContactsInfo>(DBName_BSTAM, TableName_ServiceAlert, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return obj;
        }
        /// <summary>
        /// 分页查询App联系人信息
        /// </summary>
        /// <returns></returns>
        public List<ServiceAlertContactsInfo> GetPageListServiceAlertContactsInfos(IMongoQuery query, IMongoSortBy sortBy, int pageNo, int pageSize, out long total)
        {
            total = 0;
            List<ServiceAlertContactsInfo> list = new List<ServiceAlertContactsInfo>();
            try
            {
                total = DBContext.Mongo.Count(DBName_BSTAM, TableName_ServiceAlert, query);
                list = DBContext.Mongo.GetPageList<ServiceAlertContactsInfo>(DBName_BSTAM, TableName_ServiceAlert, query, pageNo, pageSize, sortBy);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<ServiceAlertContactsInfo> GetListServiceAlertContactsInfos(IMongoQuery query)
        {
            List<ServiceAlertContactsInfo> list = new List<ServiceAlertContactsInfo>();
            try
            {
                list = DBContext.Mongo.Find<ServiceAlertContactsInfo>(DBName_BSTAM, TableName_AppContacts, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public long CheckServiceAlertContactsInfos(IMongoQuery query)
        {
            long flag = 0;
            try
            {
                flag = DBContext.Mongo.Count(DBName_BSTAM, TableName_ServiceAlert, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return flag;
        }


        /// <summary>
        /// 添加或新增App联系人信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpsertServiceAlertContactsInfos(ServiceAlertContactsInfo obj)
        {
            bool ret = false;
            if (obj == null )
            {
                return ret;
            }
            try
            {
                ret = DBContext.Mongo.Upsert<ServiceAlertContactsInfo>(DBName_BSTAM, TableName_ServiceAlert, obj);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return ret;
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="ids">联系人姓名列表</param>
        /// <returns></returns>
        public bool DelServiceAlertContactsInfos(string[] ids)
        {
            bool ret = false;
            if (ids == null || ids.Length == 0)
            {
                return ret;
            }
            try
            {
                IMongoQuery query = Query.In("_id", ids.Select(p => new BsonString(p)));
                ret = DBContext.Mongo.Remove(DBName_BSTAM, TableName_ServiceAlert, query);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return ret;
        }

        public bool SyncAlertData(int id)
        {
            bool ret = false;
            try
            {
                IMongoQuery query = Query.EQ("ServiceId", id);


                List<ServiceAlertContactsInfo> list = DBContext.Mongo.Find<ServiceAlertContactsInfo>(DBName_BSTAM, TableName_ServiceAlert, query);
                if (list.Count > 0)
                {
                    ServiceEntity model = BusinessContext.ServiceList.GetModel(id);

                    query = Query.And(Query.EQ("PrimaryId", model.PrimaryId), Query.Not(Query.EQ("_id", id)));

                    List<ServiceEntity> sList = DBContext.Mongo.Find<ServiceEntity>(DBName_BSTAM, TableName_ServiceList, query);

                    List<ServiceAlertContactsInfo> rList = new List<ServiceAlertContactsInfo>();

                    sList.ForEach(k => {
                        list.ForEach(l => {
                            ServiceAlertContactsInfo m = (ServiceAlertContactsInfo)l.Clone();
                            m._id = Guid.NewGuid().ToString();
                            m.ServiceId = Convert.ToInt32(k._id);
                            m.PrimaryId = k.PrimaryId;
                            rList.Add(m);
                        });
                    });
                    query = Query.And(Query.EQ("PrimaryId", model.PrimaryId), Query.Not(Query.EQ("ServiceId", id)));
                    DBContext.Mongo.Remove(DBName_BSTAM, TableName_ServiceAlert, query);
                    ret = DBContext.Mongo.InsertBatch(DBName_BSTAM, TableName_ServiceAlert, rList) == rList.Count;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return ret;
        }
        #endregion
    }
    #endregion
}