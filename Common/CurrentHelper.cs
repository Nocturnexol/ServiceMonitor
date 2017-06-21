using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Common;
using BS.Microservice.Web.Model;
using System.Web.Security;
using BS.Client.Common.Model;
using System.IO;
using System.Reflection;
using BS.Microservice.DTO;
using System.Collections.Concurrent;

namespace BS.Microservice.Web.Common
{
    public class CurrentHelper
    {

        /// <summary>   The service configuration. </summary>
        public static ConcurrentDictionary<string, ServiceCfg> SerCfg = new ConcurrentDictionary<string, ServiceCfg>();

        public static string ProjectKey = UseTools.GetProjectKey();
        public static string LOGIN_INFO_COOKIE = ProjectKey + "userlogininfo";
        /// <summary>
        /// 记录帐号登陆名的cookie名称
        /// </summary>
        public static string LOGIN_NAME_COOKIE = "login";
        /// <summary>
        /// 记录帐号登陆密码的cookie名称
        /// </summary>
        public static string LOGIN_PWD_COOKIE = "pwd";

        /// <summary>
        /// 
        /// </summary>
        public static string UserName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public static string UserPwd { set; get; }
        /// <summary>
        /// 
        /// </summary>
        private static Token _token;
        private static bool IsGetToken = false;
        /// <summary>
        /// 令牌
        /// </summary>
        public static Token Token
        {
            get
            {
                if (_token == null)
                {
                    _token = BS.Client.Common.CommonHelper.GetToken(UserName, UserPwd);
                }
                else
                {
                    if (!IsGetToken)
                    {
                        IsGetToken = true;
                        try
                        {
                            if ((Convert.ToDateTime(_token.ExpireTime) - DateTime.Now).TotalMinutes < 15)
                            {
                                LogManager.Info(string.Format("Token【{0}】 即将过期执行刷新Token", _token.ExpireTime));
                                Token token = BS.Client.Common.CommonHelper.RefreshToken(UserName, UserPwd, _token.RefreshToken);
                                if (token != null)
                                {
                                    _token = token;
                                    LogManager.Info(string.Format("Token【{0}】 刷新成功", _token.ExpireTime));
                                }
                                else
                                {
                                    LogManager.Info(string.Format("Token 刷新失败,重新获取Token"));
                                    _token = BS.Client.Common.CommonHelper.GetToken(UserName, UserPwd);
                                    if (_token != null)
                                    {
                                        LogManager.Info(string.Format("Token 【{0}】重新获取成功", _token.ExpireTime));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error(ex);
                        }
                        IsGetToken = false;
                    }
                }
                return _token;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets service configuration. </summary>
        ///
        /// <remarks>   Th, 2017/6/13. </remarks>
        ///
        /// <param name="serName">  Name of the ser. </param>
        /// <param name="version">  The version. </param>
        ///
        /// <returns>   The service configuration. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ServiceCfg GetServiceCfg(string serName, string version)
        {
            if (SerCfg.ContainsKey(serName))
            {
                return SerCfg[serName];
            }
            else
            {
                ServiceCfg entity = BS.Client.Common.CommonHelper.GetServiceEntity(UserName, CurrentHelper.Token.AccessToken, serName.ToString().Replace(".", "/"), version.ToString());
                return entity;
            }
        }



        static CurrentHelper()
        {
            UserName = "tanhu";
            UserPwd = "123456";
            BS.Client.Common.CommonHelper.OauthAPI = "http://61.129.57.83:21999";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo[] files = new DirectoryInfo(path).GetFiles("*.dto.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Assembly assembly = System.Reflection.Assembly.LoadFrom(file.FullName);
                List<Type> typeList = assembly.GetTypes().ToList();
                Type type = typeList.Where(p => p.Name.Equals("dtoConstants", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (type != null)
                {
                    FieldInfo ServerName = type.GetField("ServerName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (ServerName != null)
                    {
                        object serName = ServerName.GetValue(null);
                        if (serName != null && !SerCfg.ContainsKey(serName.ToString()))
                        {
                            FieldInfo VERSION = type.GetField("VERSION", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase);
                            if (VERSION != null)
                            {
                                object version = VERSION.GetValue(null);
                                if (CurrentHelper.Token != null)
                                {
                                    ServiceCfg entity = BS.Client.Common.CommonHelper.GetServiceEntity(UserName, CurrentHelper.Token.AccessToken, serName.ToString().Replace(".", "/"), version.ToString());

                                    SerCfg.AddOrUpdate(serName.ToString(), entity, (k, v) => entity);
                                }
                            }

                        }
                    }

                }

            }

        }
        /// <summary>
        /// 当前登陆的用户
        /// </summary>
        public static UserModel CurrentUser
        {
            get
            {
                //用于记录当前用户帐号信息
                UserModel m_CurrentUser = null;

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session["User"] != null)
                {
                    m_CurrentUser = (UserModel)System.Web.HttpContext.Current.Session["User"];
                }
                else if (System.Web.HttpContext.Current.Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE] != null)
                {
                    string loginName = System.Web.HttpContext.Current.Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE].Values[CurrentHelper.LOGIN_NAME_COOKIE];
                    string pwd = System.Web.HttpContext.Current.Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE].Values[CurrentHelper.LOGIN_PWD_COOKIE];

                    if (!string.IsNullOrEmpty(loginName))
                    {

                        UserEntity user = BusinessContext.User.GetModel(loginName);

                        //根据cookie当中的记录,如果验证通过则直接登陆成功
                        if (user != null && pwd == user.UserPwd)
                        {
                            FormsAuthentication.SetAuthCookie(user.LoginName, false);

                            m_CurrentUser = new UserModel();
                            m_CurrentUser.User = user;

                            System.Web.HttpContext.Current.Session["User"] = m_CurrentUser;
                        }
                    }
                }
                else if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.IsAuthenticated && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                {
                    m_CurrentUser = new UserModel();

                    m_CurrentUser.User = BusinessContext.User.GetModel(System.Web.HttpContext.Current.User.Identity.Name);

                    System.Web.HttpContext.Current.Session["User"] = m_CurrentUser;
                }

                return m_CurrentUser;
            }
            set
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Session["User"] = value;
                }
            }
        }
    }
}