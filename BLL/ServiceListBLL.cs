using System.Collections.Generic;
using System.Web.Mvc;
using BS.Microservice.Web.DAL;
using BS.Microservice.Web.Model;
using BS.Common.Model.Mongo.ServiceModels;
using MongoDB.Driver;
using ServiceEntity = BS.Microservice.Web.Model.ServiceEntity;

namespace BS.Microservice.Web.BLL
{
    public class ServiceListBLL
    {
        private readonly ServiceListDAL _dal = new ServiceListDAL();

        public bool Exists(string userName)
        {
            return _dal.Exists(userName);
        }
        public bool Exists(string userName, int _id)
        {
            return _dal.Exists(userName, _id);
        }

        public bool Exists(string serName, string secName)
        {
            return _dal.Exists(serName, secName);
        }
        public bool Update(ServiceEntity model)
        {
            return _dal.Update(model);
        }
        public bool Add(ServiceEntity model)
        {
            return _dal.Add(model);
        }
        public List<TreeModel> GetTreeModels(ServiceTypeEnum? type=null)
        {
            return _dal.GetTreeModels(type);
        }

        public bool Delete(int id)
        {
            return _dal.Delete(id);
        }
        public List<ServiceEntity> GetList(IMongoQuery query)
        {
            return _dal.GetList(query);
        }
        public ServiceEntity GetModel(IMongoQuery query)
        {
            return _dal.GetModel(query);
        }
        public ServiceEntity GetModel(int id)
        {
            return _dal.GetModel(id);
        }
        public ServiceEntity GetModel(string userName)
        {
            return _dal.GetModel(userName);
        }

        public IList<SelectListItem> GetHostList(ServiceTypeEnum? type=null)
        {
            return _dal.GetHostList(type);
        }
        public List<ServiceEntity> GetModelList(ServiceTypeEnum? type, string orderBy, string desc, int page, int pageSize,string id,string keyword,string isApproved,string host,out int count)
        {
            return _dal.GetModelList(type, orderBy, desc, page, pageSize, id, keyword, isApproved, host,out count);
        }

        public int GetCount(ServiceTypeEnum? type = null)
        {
            return _dal.GetCount(type);
        }
    }
}