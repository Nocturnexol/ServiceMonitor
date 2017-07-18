using MongoDB.Bson;

namespace BS.Microservice.Web.Model
{
    public class tblUser_Roles
    {
        public ObjectId _id { get; set; }
        #region Model
        private int rid;
        private string _loginname;
        private int _role_id;
        private bool _isDefault;
        private string _remark;
        /// <summary>
        /// 
        /// </summary>
        public int Rid
        {
            set { rid = value; }
            get { return rid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LoginName
        {
            set { _loginname = value; }
            get { return _loginname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Role_Id
        {
            set { _role_id = value; }
            get { return _role_id; }
        }
        public bool IsDefault
        {
            set { _isDefault = value; }
            get { return _isDefault; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        #endregion Model
    }
}