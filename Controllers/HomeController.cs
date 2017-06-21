using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BS.Common;
using BS.Microservice.Web.Model;
using System.Security.Cryptography;
using System.Text;
using BS.Microservice.Web.Common;
using System.Web.Security;

namespace BS.Microservice.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (CurrentHelper.CurrentUser == null || CurrentHelper.CurrentUser.User == null)
            {
                return LogOut();
            }


            return View();
        }
        public ActionResult MainPage()
        {
            if (CurrentHelper.CurrentUser == null || CurrentHelper.CurrentUser.User == null)
            {
                return LogOut();
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                if (Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE] != null)
                {
                    string loginName = CurrentHelper.CurrentUser.User.LoginName;
                    string pwd = Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE].Values[CurrentHelper.LOGIN_PWD_COOKIE];
                    if (!string.IsNullOrEmpty(loginName))
                    {
                        UserEntity user = BusinessContext.User.GetModel(loginName);
                        //根据cookie当中的记录,如果验证通过则直接登陆成功
                        if (user != null && pwd == user.UserPwd)
                        {
                            #region 初始化用户对象
                            UserModel m_CurrentUser = new UserModel();
                            m_CurrentUser.User = user;
                           
                            System.Web.HttpContext.Current.Session["User"] = m_CurrentUser;
                            #endregion

                            if (this.Request.RawUrl != this.Request.Url.AbsolutePath && !this.Request.RawUrl.ToLower().Contains("/home/login"))
                            {
                                FormsAuthentication.SetAuthCookie(user.LoginName, false);
                                return Redirect(this.Request.RawUrl);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(FormsAuthentication.GetRedirectUrl(user.LoginName, false)))
                                {
                                    FormsAuthentication.RedirectFromLoginPage(user.LoginName, true);
                                }
                                else
                                {
                                    return Redirect(FormsAuthentication.DefaultUrl);
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(Request["LoginName"]) && !string.IsNullOrWhiteSpace(Request["UserPwd"]))
                {
                    string loginName = Request["LoginName"];
                    string pwd = Request["UserPwd"];
                    UserEntity user = BusinessContext.User.GetModel(loginName);

                    if (user != null && pwd == user.UserPwd)
                    {
                        #region 初始化用户对象
                        UserModel m_CurrentUser = new UserModel();
                        m_CurrentUser.User = user;
                      

                        System.Web.HttpContext.Current.Session["User"] = m_CurrentUser;
                        #endregion

                        if (this.Request.RawUrl != this.Request.Url.AbsolutePath && !this.Request.RawUrl.ToLower().Contains("/home/login"))
                        {
                            FormsAuthentication.SetAuthCookie(user.LoginName, false);
                            return Redirect(this.Request.RawUrl);
                        }
                        else
                        {
                            FormsAuthentication.RedirectFromLoginPage(user.LoginName, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("登录异常", ex);
                return LogOut();
            }

            return View();
        }
        #region 注销
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns>转向到首页</returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            ///移除会话
            System.Web.HttpContext.Current.Session.Remove("User");
            CurrentHelper.CurrentUser = null;
            if (Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE] != null)
            {
                ///Cookie置为过期
                Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE].Expires = DateTime.Now.AddDays(-1);
                Response.AppendCookie(Request.Cookies[CurrentHelper.LOGIN_INFO_COOKIE]);
            }
            ///转到登录页面
            return RedirectToAction("Login");
        }
        #endregion
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="form">获取页面表单元素的数据</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(FormCollection form)
        {
            string loginName = Convert.ToString(form["LoginName"]);
            string passWord = Convert.ToString(form["PassWord"]);
            try
            {
                UserEntity user = BusinessContext.User.GetModel(loginName);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString((string)Session["private_key"]);
                byte[] result = rsa.Decrypt(passWord.ToHexBytes(), false);
                passWord = Encoding.UTF8.GetString(result);
                if (user != null && string.Compare(user.UserPwd, passWord, true) == 0)
                {

                    HttpCookie userlogininfo = new HttpCookie(CurrentHelper.LOGIN_INFO_COOKIE);
                    userlogininfo.HttpOnly = true;
                    userlogininfo.Expires = DateTime.Now.AddHours(2);//Cookie存活2小时
                    userlogininfo.Values.Add(CurrentHelper.LOGIN_NAME_COOKIE, user.LoginName);
                    userlogininfo.Values.Add(CurrentHelper.LOGIN_PWD_COOKIE, user.UserPwd);
                    Response.AppendCookie(userlogininfo);
                    #region 初始化用户对象
                    UserModel m_CurrentUser = new UserModel();
                    m_CurrentUser.User = user;

                    if (user.FailTimes > 0)
                    {
                        user.FailTimes = 0;
                        BusinessContext.User.Update(user);
                    }
                    System.Web.HttpContext.Current.Session["User"] = m_CurrentUser;
                    #endregion
                    ReturnMessage RM = new ReturnMessage(true);
                    return Json(RM, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (user != null && user._id > 0)
                    {
                        user.FailTimes++;
                        BusinessContext.User.Update(user);
                    }
                    ReturnMessage RM = new ReturnMessage(false);
                  
                    RM.Message = "登录名或密码错误！";
                    return Json(RM, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("登录异常", ex);
                ReturnMessage RM = new ReturnMessage(false);
               
                RM.Message = "异常,请重试！";
                return Json(RM, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 登录令牌
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult getSecurityToken()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameter = rsa.ExportParameters(true);
            //将私钥存Session中
            Session["private_key"] = rsa.ToXmlString(true);
            string exponent = parameter.Exponent.ToHexString();
            string modulus = parameter.Modulus.ToHexString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Session["exponent"] = exponent;
            dic.Add("exponent", exponent);
            dic.Add("modulus", modulus);
            Session["modulus"] = modulus;
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
    }
}
