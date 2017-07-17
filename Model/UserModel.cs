using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace BS.Microservice.Web.Model
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public class UserModel
    {
        public UserEntity User { get; set; }
        public List<sys_role> Roles { get; set; }
    }
    /// <summary>
    /// 基类
    /// </summary>
    public partial class BaseModel
    {
        public ObjectId _id { get; set; }
        public int Rid { get; set; }

        public bool IsDel { get; set; }
    }
}