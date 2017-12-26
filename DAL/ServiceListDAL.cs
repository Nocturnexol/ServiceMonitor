using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BS.Microservice.Web.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using BS.Microservice.Web.Common;
using BS.Common;
using BS.Common.Model.Mongo.ServiceModels;
using MongoDB.Bson;
using ServiceEntity = BS.Microservice.Web.Model.ServiceEntity;

namespace BS.Microservice.Web.DAL
{
    public class ServiceListDAL
    {
        private const string Col = "ServiceList";

        //public ServiceListDAL()
        //{
        //    Convert.ToInt32(DBContext.Mongo.GetScalar(DBContext.DbName, COL));
        //}
        public bool Exists(string userName)
        {
            var query = Query.EQ("UserName", userName);
            return DBContext.Mongo.Count(DBContext.DbName, Col, query) > 0;

        }
        public bool Exists(string userName,int id)
        {
            var query = Query.And(Query.EQ("UserName", userName), Query.NE("_id", id));
            return DBContext.Mongo.Count(DBContext.DbName, Col, query) > 0;
        }

        public bool Exists(string serName, string secName)
        {
            var query = Query.And(Query.EQ("ServiceName", serName), Query.EQ("SecondaryName", secName));
            return DBContext.Mongo.Count(DBContext.DbName, Col, query) > 0;
        }

        public List<TreeModel> GetTreeModels(ServiceTypeEnum? type)
        {
            var res = new List<TreeModel>
            {
                new TreeModel {id = "0", parent = "#", text = "全部"}
            };
            var col = DBContext.Mongo.GetMongoDB(DBContext.DbName, true)
                .GetCollection(Col);
            var tree =
                col.FindAs<ServiceEntity>(type.HasValue ? Query<ServiceEntity>.EQ(t => t.ServiceType, type.Value) : null)
                    .Select(t => new {t._id, t.ServiceName, t.SecondaryName}).ToList();
            var pTree =
                tree.GroupBy(t => t.ServiceName)
                    .Select(t => new TreeModel {id = "1_" + t.Key, parent = "0", text = t.Key}).ToList();
            var sec = tree.GroupBy(t => t.SecondaryName).ToList();
            var sTree = (from s in sec
                let first = tree.Where(x => x.SecondaryName == s.Key).GroupBy(x => new {x.SecondaryName, x.ServiceName})
                from f in first
                select new TreeModel
                {
                    id = "2_" + s.Key + "_" + f.Key.ServiceName,
                    parent = "1_" + f.Key.ServiceName,
                    text = s.Key
                }).ToList();


            //var sTree =
            //    tree.GroupBy(t => t.SecondaryName)
            //        .Select(
            //            t =>
            //                new TreeModel
            //                {
            //                    id = "2_" + t.Key,
            //                    parent = "1_" + tree.First(x => x.SecondaryName == t.Key).ServiceName,
            //                    text = t.Key
            //                });
            //var p = DBContext.Mongo.Distinct(DBContext.DbName, COL, "ServiceName");
            //var pTree = p.Select((t, i) => new TreeModel {id = "1_" + t, parent = "1", text = t.ToString()});
            //var s = DBContext.Mongo.Distinct(DBContext.DbName, COL, "SecondaryName");
            //var sTree = s.Select((t, i) => new TreeModel { id = "2_" + t, parent = "2", text = t.ToString() });


            var names = DBContext.Mongo.GetMongoDB(DBContext.DbName, true)
                .GetCollection("GroupName")
                .FindAs<GroupName>(Query<GroupName>.In(t => t.ServiceName, pTree.Select(x => x.text)))
                .ToList();
            res.AddRange(names.Join(pTree, g => g.ServiceName, t => t.text,
                (g, t) => new TreeModel {id = t.id, parent = t.parent, text = g.ServiceNameCN}));
            res.AddRange(sTree);
            return res;
        }

        public bool Add(ServiceEntity model)
        {
            model._id = Convert.ToInt32(DBContext.Mongo.GetScalar(DBContext.DbName, Col)) + 1;

            var service =
                DBContext.Mongo.GetMongoDB(DBContext.DbName, true)
                    .GetCollection("GroupName")
                    .FindAs<GroupName>(Query<GroupName>.EQ(t => t.ServiceName, model.ServiceName))
                    .ToList();
            if (service.Any())
            {
                model.PrimaryId = service.First()._id;
            }

            var cursor = DBContext.Mongo.GetMongoDB(DBContext.DbName, true).GetCollection(Col).FindAllAs<ServiceEntity>();
            cursor.SetSortOrder(SortBy.Descending("SecondaryId"));
            var first = cursor.FirstOrDefault();

            model.SecondaryId = first != null ? first.SecondaryId + 1 : 1;

            return DBContext.Mongo.Insert(DBContext.DbName,Col,model);
        }

        public bool Update(ServiceEntity model)
        {
            var service =
                DBContext.Mongo.GetMongoDB(DBContext.DbName, true)
                    .GetCollection("GroupName")
                    .FindAs<GroupName>(Query<GroupName>.EQ(t => t.ServiceName, model.ServiceName))
                    .ToList();
            if (service.Any())
            {
                model.PrimaryId = service.First()._id;
            }
            return DBContext.Mongo.Upsert(DBContext.DbName, Col, model);
        }
       
        public bool Delete(int id)
        {
            var query = Query.EQ("_id", id);
            return DBContext.Mongo.Remove(DBContext.DbName, Col, query);
        }

        public List<ServiceEntity> GetList(IMongoQuery query)
        {
            return DBContext.Mongo.Find<ServiceEntity>(DBContext.DbName, Col, query);
        }
        public ServiceEntity GetModel(IMongoQuery query)
        {
            return DBContext.Mongo.FindOne<ServiceEntity>(DBContext.DbName, Col, query);
        }
        public ServiceEntity GetModel(int id)
        {
            var query = Query.EQ("_id", id);
            return DBContext.Mongo.FindOne<ServiceEntity>(DBContext.DbName, Col, query);
        }
        public ServiceEntity GetModel(string userName)
        {
            var query = Query.EQ("LoginName", userName);
            return DBContext.Mongo.FindOne<ServiceEntity>(DBContext.DbName, Col, query);
        }

        public IList<SelectListItem> GetHostList(ServiceTypeEnum? type)
        {
            var query = type.HasValue ? Query<ServiceEntity>.EQ(t => t.ServiceType, type.Value) : null;
            return DBContext.Mongo.Distinct(DBContext.DbName, Col, "Host", query).Select(t => new SelectListItem { Text = t.ToString(), Value = t.ToString() }).ToList();
        }

        public List<ServiceEntity> GetModelList(ServiceTypeEnum? type, string orderBy, string desc, int page,
            int pageSize, string id, string keyword, string isApproved, string host, out int count)
        {
            count = 0;
            try
            {
                var queryList = new List<IMongoQuery>();
                if (type.HasValue)
                {
                    queryList.Add(Query<ServiceEntity>.EQ(t => t.ServiceType, type.Value));
                }
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var arr = id.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                    if (arr[0] == "1")
                    {
                        queryList.Add(Query<ServiceEntity>.EQ(t => t.ServiceName, arr[1]));
                    }
                    else if (arr[0] == "2")
                    {
                        var q = Query<ServiceEntity>.EQ(t => t.SecondaryName, arr[1]);
                        queryList.Add(arr.Length > 2
                            ? Query.And(q, Query<ServiceEntity>.EQ(t => t.ServiceName, arr[2]))
                            : q);
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
                if (!string.IsNullOrWhiteSpace(isApproved))
                {
                    int approved;
                    if (int.TryParse(isApproved, out approved))
                        if (approved > 0)
                            queryList.Add(Query<ServiceEntity>.EQ(t => t.IsApproved, approved == 1));
                }
                if (!string.IsNullOrWhiteSpace(host))
                {
                    queryList.Add(Query<ServiceEntity>.EQ(t => t.Host, host));
                }
                IMongoSortBy sortBy = SortBy.Ascending(orderBy);
                if (desc.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    sortBy = SortBy.Descending(orderBy);
                }
                var query = queryList.Any() ? Query.And(queryList) : null;
                count = (int) DBContext.Mongo.Count(DBContext.DbName, Col, query);
                var res = DBContext.Mongo.GetPageList<ServiceEntity>(DBContext.DbName, Col, query, page, pageSize,
                    sortBy, null);
                return res;

            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            return new List<ServiceEntity>();
        }

        public int GetCount(ServiceTypeEnum? type)
        {
            var query = type.HasValue ? Query<ServiceEntity>.EQ(t => t.ServiceType, type.Value) : null;
            return (int)DBContext.Mongo.Count(DBContext.DbName, Col, query);
        }
    }
}