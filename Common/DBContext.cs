using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.DB;
using System.Configuration;

namespace BS.Microservice.Web.Common
{
    public class DBContext
    {
        public static MongoDBVisitor Mongo;
        public static string DbName = "Microservice";
        static DBContext()
        {
            string host = ConfigurationManager.AppSettings["mongo"];
            Mongo = DBFactory.CreateMongoDBAccess(host);
        }
    }
}