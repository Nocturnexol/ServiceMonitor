using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BS.Microservice.Web.Model
{
    public class tblDepart
    {
        public ObjectId _id { get; set; }
        #region Model

        /// <summary>
        /// 
        /// </summary>
        public int Rid { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string dept { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string remark { set; get; }

        #endregion Model
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreateOn { get; set; }
        public string CreateBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}