using MongoDB.Bson;

namespace BS.Microservice.Web.Model
{
    public class tblDepart
    {
        public ObjectId _id { get; set; }
        #region Model
        private int rid;
        private string _dept;
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
        public string dept
        {
            set { _dept = value; }
            get { return _dept; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        #endregion Model
    }
}