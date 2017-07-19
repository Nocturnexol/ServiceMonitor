using BS.Microservice.Web.BLL;
using BS.Microservice.Web.Model;

namespace BS.Microservice.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessContext
    {
        /// <summary>
        /// User业务处理类
        /// </summary>
        public static UserBLL User
        {
            get
            {
                if (_User == null)
                {
                    _User = new UserBLL();
                }
                return _User;
            }
        }
        private static UserBLL _User;


        /// <summary>
        /// ServiceList业务处理类
        /// </summary>
        public static ServiceListBLL ServiceList
        {
            get
            {
                if (_ServiceList == null)
                {
                    _ServiceList = new ServiceListBLL();
                }
                return _ServiceList;
            }
        }
        private static ServiceListBLL _ServiceList;

        /// <summary>
        /// FunctionalAuthority业务处理类
        /// </summary>
        public static MongoBll<FunctionalAuthority> FunctionalAuthority
        {
            get { return _functionalAuthority ?? (_functionalAuthority = new MongoBll<FunctionalAuthority>()); }
        }
        private static MongoBll<FunctionalAuthority> _functionalAuthority;

        /// <summary>
        /// sys_role_right业务处理类
        /// </summary>
        public static MongoBll<sys_role_right> sys_role_right
        {
            get { return _sys_role_right ?? (_sys_role_right = new MongoBll<sys_role_right>()); }
        }
        private static MongoBll<sys_role_right> _sys_role_right;


        /// <summary>
        /// tblGroupButton业务处理类
        /// </summary>
        public static MongoBll<tblGroupButton> tblGroupButton
        {
            get { return _tblGroupButton ?? (_tblGroupButton = new MongoBll<tblGroupButton>()); }
        }
        private static MongoBll<tblGroupButton> _tblGroupButton;

        /// <summary>
        /// tblButtonName业务处理类
        /// </summary>
        public static MongoBll<tblButtonName> tblButtonName
        {
            get { return _tblButtonName ?? (_tblButtonName = new MongoBll<tblButtonName>()); }
        }
        private static MongoBll<tblButtonName> _tblButtonName;

        public static MongoBll<tblDepart> tblDepart
        {
            get { return _tblDepart ?? (_tblDepart = new MongoBll<tblDepart>()); }
        }
        private static MongoBll<tblDepart> _tblDepart;

        /// <summary>
        /// sys_role业务处理类
        /// </summary>
        public static MongoBll<sys_role> sys_role
        {
            get { return _sys_role ?? (_sys_role = new MongoBll<sys_role>()); }
        }
        private static MongoBll<sys_role> _sys_role;

        /// <summary>
        /// tblUser_Roles业务处理类
        /// </summary>
        public static MongoBll<tblUser_Roles> tblUser_Roles
        {
            get
            {
                if (_tblUser_Roles == null)
                {
                    _tblUser_Roles = new MongoBll<tblUser_Roles>();
                }
                return _tblUser_Roles;
            }
        }
        private static MongoBll<tblUser_Roles> _tblUser_Roles;

        public static MongoBll<GroupName> GroupName
        {
            get { return _groupName ?? (_groupName = new MongoBll<GroupName>()); }
        }
        private static MongoBll<GroupName> _groupName;

        public static MongoBll<FileEntity> Files
        {
            get { return _files ?? (_files = new MongoBll<FileEntity>()); }
        }
        private static MongoBll<FileEntity> _files;
    }
}