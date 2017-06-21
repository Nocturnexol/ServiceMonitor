using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Microservice.Web.Model
{
    [Serializable]
    public partial class UserEntity
    {
        public UserEntity()
        { }
        #region Model
        public int _id { set; get; }
        private string _loginname;
        private string _username;
        private string _englishname;
        private string _usermark;
        private string _dept;
        private string _dept_new;
        private string _userpwd;
        private string _password;
        private string _remark;

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
        public string UserName
        {
            set { _username = value; }
            get { return _username; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EnglishName
        {
            set { _englishname = value; }
            get { return _englishname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserMark
        {
            set { _usermark = value; }
            get { return _usermark; }
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
        public string dept_New
        {
            set { _dept_new = value; }
            get { return _dept_new; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserPwd
        {
            set { _userpwd = value; }
            get { return _userpwd; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PassWord
        {
            set { _password = value; }
            get { return _password; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailTimes { set; get; }
        #endregion Model

    }
}