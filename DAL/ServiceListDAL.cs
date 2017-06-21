using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using BS.Microservice.Web.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using BS.Microservice.Web.Common;
using BS.Common;
using BS.Common.Model.Mongo.ServiceModels;
using MongoDB.Bson;

namespace BS.Microservice.Web.DAL
{
    public class ServiceListDAL
    {
        const string COL = "ServiceList";
        int MaxId = 1;
        public ServiceListDAL()
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

        public bool Exists(string serName, string secName)
        {
            IMongoQuery query = Query.And(Query.EQ("ServiceName", serName), Query.EQ("SecondaryName", secName));
            return DBContext.Mongo.Count(DBContext.DbName, COL, query) > 0;
        }

        public List<TreeModel> GetTreeModels()
        {
            var res = new List<TreeModel>
            {
                new TreeModel {id = "0", parent = "#", text = "全部"},
                new TreeModel {id = "1", parent = "#", text = "一级服务"},
                new TreeModel {id = "2", parent = "#", text = "二级服务"}
            };
            var p = DBContext.Mongo.Distinct(DBContext.DbName, COL, "ServiceName");
            var pTree = p.Select((t, i) => new TreeModel {id = "1_" + t, parent = "1", text = t.ToString()});
            var s = DBContext.Mongo.Distinct(DBContext.DbName, COL, "SecondaryName");
            var sTree = s.Select((t, i) => new TreeModel { id = "2_" + t, parent = "2", text = t.ToString() });
            res.AddRange(pTree);
            res.AddRange(sTree);
            return res;
        }

        public bool Add(ServiceEntity model)
        {
            model._id = Convert.ToInt32(DBContext.Mongo.GetScalar(DBContext.DbName, COL)) + 1;
            return DBContext.Mongo.Insert(DBContext.DbName,COL,model);
        }

        public bool Update(ServiceEntity model)
        {
            return DBContext.Mongo.Upsert(DBContext.DbName, COL, model);
        }
       
        public bool Delete(int _id)
        {
            IMongoQuery query = Query.EQ("_id", _id);
            return DBContext.Mongo.Remove(DBContext.DbName, COL, query);
        }

        public ServiceEntity GetModel(int _id)
        {
            IMongoQuery query = Query.EQ("_id", _id);
            return DBContext.Mongo.FindOne<ServiceEntity>(DBContext.DbName, COL, query);
        }
        public ServiceEntity GetModel(string userName)
        {
            IMongoQuery query = Query.EQ("LoginName", userName);
            return DBContext.Mongo.FindOne<ServiceEntity>(DBContext.DbName, COL, query);
        }
        public List<ServiceEntity> GetModelList(Dictionary<string, string> where, string orderBy, string desc, int page, int pageSize,string id,string keyword)
        {
            try
            {
                var queryList = new List<IMongoQuery>();
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var arr = id.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr[0] == "1")
                    {
                        queryList.Add(Query<ServiceEntity>.EQ(t => t.ServiceName, arr[1]));
                    }
                    else if (arr[0] == "2")
                    {
                        queryList.Add(Query<ServiceEntity>.EQ(t => t.SecondaryName, arr[1]));
                    }
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    queryList.Add(Query.Or(Query<ServiceEntity>.Matches(t => t.ServiceName,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))),
                        Query<ServiceEntity>.Matches(t => t.SecondaryName,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))),
                        Query<ServiceEntity>.Matches(t => t.Host,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))),
                        Query<ServiceEntity>.Matches(t => t.RegContent,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))),
                        Query<ServiceEntity>.Matches(t => t.Version,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase))),
                        Query<ServiceEntity>.Matches(t => t.Remark,
                            new BsonRegularExpression(new Regex(keyword, RegexOptions.IgnoreCase)))));
                }
                IMongoSortBy sortBy = SortBy.Ascending(orderBy);
                if (desc.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    sortBy = SortBy.Descending(orderBy);
                }
                var query = queryList.Any() ? Query.And(queryList) : null;
                return DBContext.Mongo.GetPageList<ServiceEntity>(DBContext.DbName, COL, query, page, pageSize, sortBy, null);

            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return new List<ServiceEntity>();
        }

        public int GetCount(Dictionary<string, string> dic)
        {
            IMongoQuery query = null;
            return (int)DBContext.Mongo.Count(DBContext.DbName, COL, query);
        }
    }
}