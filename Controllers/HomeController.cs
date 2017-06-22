using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BS.Common;
using BS.Microservice.Web.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

        public FileStreamResult DownFile(string filePath, string fileName)
        {
            filePath = Request["filePath"];
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "Excel.xlsx";
            //删除10分钟之前的数据
            string dir = filePath.Substring(0, filePath.LastIndexOf("\\"));

            FileStream file = new System.IO.FileStream(filePath, System.IO.FileMode.Open);

            DelOverdueFile(dir, 10);
            return File(file, "application/octet-stream", (file.Name.Substring(filePath.LastIndexOf("\\") + 1)));
        }
        /// <summary>
        /// 删除当前目录下n天之前(按文件最后写入时间)的历史数据文件
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="searchPattert">搜索模式(如：*.log),为空则获取全部文件</param>
        /// <param name="OverDay">过期分钟</param>
        public static void DelOverdueFile(string dir, int OverDay)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);
                System.IO.DirectoryInfo[] dirs = di.GetDirectories();
                System.IO.FileInfo[] files = di.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
                DateTime now = DateTime.Now;
                System.Collections.Generic.List<System.IO.DirectoryInfo> newList = new System.Collections.Generic.List<System.IO.DirectoryInfo>();

                if (dirs.Length > 0)
                {
                    foreach (var item in dirs)
                    {
                        if ((now - item.LastWriteTime).TotalMinutes > OverDay)
                        {
                            newList.Add(item);
                        }
                    }
                }
                if (files.Length > 0)
                {
                    foreach (var f in files)
                    {
                        if ((now - f.LastWriteTime).TotalMinutes > OverDay && Regex.IsMatch(f.Name, @"\d"))
                        {
                            f.Delete();
                        }
                    }
                }
                foreach (var f in newList)
                {
                    DelDirectory(f);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 递归删除目录下所有子目录和文件到目标路径
        /// </summary>
        /// <param name="sPath">源目录</param>
        /// <param name="dPath">目的目录</param>
        public static void DelDirectory(System.IO.DirectoryInfo dirInfo)
        {
            // 目录为空则返回
            if (dirInfo == null)
            {
                return;
            }
            try
            {
                System.IO.DirectoryInfo[] directories = dirInfo.GetDirectories();
                if (directories.Length > 0)
                {
                    foreach (System.IO.DirectoryInfo temDirectoryInfo in directories)
                    {
                        DelDirectory(temDirectoryInfo);
                    }
                }
                dirInfo.Delete();
            }
            catch
            {

            }
        }
    }
}
