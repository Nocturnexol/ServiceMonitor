using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Microservice.Web.Model
{
    public class FunctionalAuthority
    {
        public FunctionalAuthority()
        { }
        #region Model
        private int _tblrcdid;
        private string _module_name;
        private int _module_id = 1;
        private string _group_name;
        private int _group_id = 1;
        private string _right_name;
        private decimal _right_id;
        private string _rigth_url;
        private string _rigth_tip;
        private string _remark;
        /// <summary>
        /// 
        /// </summary>
        public int TblRcdId
        {
            set { _tblrcdid = value; }
            get { return _tblrcdid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Module_Name
        {
            set { _module_name = value; }
            get { return _module_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Module_Id
        {
            set { _module_id = value; }
            get { return _module_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Group_Name
        {
            set { _group_name = value; }
            get { return _group_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Group_Id
        {
            set { _group_id = value; }
            get { return _group_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Right_Name
        {
            set { _right_name = value; }
            get { return _right_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Right_Id
        {
            set { _right_id = value; }
            get { return _right_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Rigth_Url
        {
            set { _rigth_url = value; }
            get { return _rigth_url; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Rigth_Tip
        {
            set { _rigth_tip = value; }
            get { return _rigth_tip; }
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