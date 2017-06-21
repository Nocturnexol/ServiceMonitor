using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BS.Microservice.Web.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using BS.Microservice.Web.Common;
using BS.Common;

namespace BS.Microservice.Web.DAL
{
    public class UserDAL
    {
        const string COL = "User";
        int MaxId = 1;
        public UserDAL()
        {
            MaxId = Convert.ToInt32(DBContext.Mongo.GetScalar(DBContext.DbName, COL));
        }
        public bool Exists(string userName)
        {
            IMongoQuery query = Query.EQ("UserName", userName);
            return DBContext.Mongo.Count(DBContext.DbName, COL, query) > 0;

        }
        public bool Exists(string userName,int _id)
        {
            IMongoQuery query = Query.And(Query.EQ("UserName", userName), Query.NE("_id", _id));
            return DBContext.Mongo.Count(DBContext.DbName, COL, query) > 0;
        }
        public bool Add(UserEntity model)
        {
            if (model._id <= MaxId)
            {
                model._id = MaxId++;
            }
            return DBContext.Mongo.Insert(DBContext.DbName, COL, model);
        }

        public bool Update(UserEntity model)
        {
            return DBContext.Mongo.Upsert(DBContext.DbName, COL, model);
        }

        public bool Delete(int _id)
        {
            IMongoQuery query = Query.EQ("_id", _id);
            return DBContext.Mongo.Remove(DBContext.DbName, COL, query);
        }

        public UserEntity GetModel(int _id)
        {
            IMongoQuery query = Query.EQ("_id", _id);
            return DBContext.Mongo.FindOne<UserEntity>(DBContext.DbName, COL, query);
        }
        public UserEntity GetModel(string userName)
        {
            IMongoQuery query = Query.EQ("LoginName", userName);
            return DBContext.Mongo.FindOne<UserEntity>(DBContext.DbName, COL, query);
        }
        public List<UserEntity> GetModelList(Dictionary<string, string> where, string orderBy, string desc, int page, int pageSize)
        {
            try
            {
                IMongoQuery query = null;
                IMongoSortBy sortBy = SortBy.Ascending(orderBy);
                if (desc.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    sortBy = SortBy.Descending(orderBy);
                }

                return DBContext.Mongo.GetPageList<UserEntity>(DBContext.DbName, COL, query, page, pageSize, sortBy, null);

            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return new  List<UserEntity>();
        }

        public int GetCount(Dictionary<string, string> dic)
        {
            IMongoQuery query = null;
            return (int)DBContext.Mongo.Count(DBContext.DbName, COL, query);
        }
    }
}