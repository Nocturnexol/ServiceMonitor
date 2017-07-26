using MongoDB.Bson;

namespace BS.Microservice.Web.Model
{
    public class BasicType
    {
        public ObjectId _id { get; set; }
        public int Rid { get; set; }
        public int? TypeId { get; set; }
        public int? Num { get; set; }
        public string Name { get; set; }
    }
}