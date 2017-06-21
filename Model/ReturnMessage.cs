using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace BS.Microservice.Web.Model
{
    /// <summary>
    /// 返回消息类
    /// </summary>
    public class ReturnMessage
    {
        private IDictionary<string, object> m_Data = new Dictionary<string, object>();
        /// <summary>
        /// 响应结果码
        /// 1:成功 0:失败
        /// </summary>
        public string resultCode { get; set; }

        /// <summary>
        /// 响应结果码
        /// 1:成功 0:失败
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 若请求失败，此处记录失败原因，若resultCode为1，此处可以为空
        /// </summary>
        public string resultDesc { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public object result;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="IsSuccess">默认是true还是false</param>
        public ReturnMessage(bool IsSuccess)
        {
            this.IsSuccess = IsSuccess;
        }

        public bool IsContinue { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReturnMessage()
        {

        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string RedirectUrl { set; get; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回单项数据信息
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 返多项值,以字典形式返回
        /// </summary>
        public IDictionary<string, object> ResultData
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// ToJSONString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return JsonConvert.SerializeObject(this);
        }
    }

    public class TreeModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
    }

}