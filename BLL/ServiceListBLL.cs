﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Microservice.Web.DAL;
using BS.Microservice.Web.Model;
using BS.Common.Model.Mongo.ServiceModels;

namespace BS.Microservice.Web.BLL
{
    public class ServiceListBLL
    {
        private readonly ServiceListDAL dal = new ServiceListDAL();
        public ServiceListBLL()
        {
        }
        public bool Exists(string userName)
        {
            return dal.Exists(userName);
        }
        public bool Exists(string userName, int _id)
        {
            return dal.Exists(userName, _id);
        }

        public bool Exists(string serName, string secName)
        {
            return dal.Exists(serName, secName);
        }
        public bool Update(ServiceEntity model)
        {
            return dal.Update(model);
        }
        public bool Add(ServiceEntity model)
        {
            return dal.Add(model);
        }
        public List<TreeModel> GetTreeModels()
        {
            return dal.GetTreeModels();
        }

        public bool Delete(int _id)
        {
            return dal.Delete(_id);
        }

        public ServiceEntity GetModel(int _id)
        {
            return dal.GetModel(_id);
        }
        public ServiceEntity GetModel(string userName)
        {
            return dal.GetModel(userName);
        }
        public List<ServiceEntity> GetModelList(Dictionary<string, string> dic, string orderBy, string desc, int page, int pageSize,string id,string keyword)
        {
            return dal.GetModelList(dic, orderBy, desc, page, pageSize,id,keyword);
        }

        public int GetCount(Dictionary<string, string> dic)
        {
            return dal.GetCount(dic);
        }
    }
}