using System.Configuration;
using BS.DB;

namespace BS.Microservice.Web.Common
{
    public class DBContext
    {
        public static MongoDBVisitor Mongo;
        public static string DbName = "Microservice";
        public static string Host = ConfigurationManager.AppSettings["mongo"];
        static DBContext()
        {
            Mongo = DBFactory.CreateMongoDBAccess(Host);
        }
    }
}