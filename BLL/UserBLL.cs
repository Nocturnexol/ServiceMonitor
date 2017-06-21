using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Microservice.Web.DAL;
using BS.Microservice.Web.Model;

namespace BS.Microservice.Web.BLL
{
    public class UserBLL
    {
        private readonly UserDAL dal = new UserDAL();
        public UserBLL()
        {
        }
        public bool Exists(string userName)
        {
            return dal.Exists(userName);
        }
        public bool Exists(string userName,int _id)
        {
            return dal.Exists(userName, _id);
        }
        public bool Add(UserEntity model)
        {

            return dal.Add(model);
        }

        public bool Update(UserEntity model)
        {
            return dal.Update(model);
        }

        public bool Delete(int _id)
        {
            return dal.Delete(_id);
        }

        public UserEntity GetModel(int _id)
        {
            return dal.GetModel(_id);
        }
        public UserEntity GetModel(string userName)
        {
            return dal.GetModel(userName);
        }
        public List<UserEntity> GetModelList(Dictionary<string, string> dic, string orderBy, string desc, int page, int pageSize)
        {
            return dal.GetModelList(dic, orderBy, desc, page, pageSize);
        }

        public int GetCount(Dictionary<string, string> dic)
        {
            return dal.GetCount(dic);
        }
    }
}