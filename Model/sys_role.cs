using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BS.Microservice.Web.Model
{
    public class sys_role:BaseModel
    {
        #region Model

        /// <summary>
        /// 
        /// </summary>
        public string role_name { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string role_desc { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int? role_flag { set; get; }

        #endregion Model
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreateOn { get; set; }
        public string CreateBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}