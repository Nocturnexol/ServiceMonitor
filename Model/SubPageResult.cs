using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Microservice.Web.Model
{
    /// <summary>
    ///分页数据 
    /// </summary>
    public class SubPageResult<T> where T : new()
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 分页数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int records { get; set; }
        /// <summary>
        /// 结果集
        /// </summary>
        public List<T> rows { get; set; }

        /// <summary>
        /// 汇总
        /// </summary>
        public List<KeyValue> summary { get; set; }


        /// <summary>
        /// 汇总跑马灯效果
        /// </summary>
        public List<KeyValue> horselamp { get; set; }
    }

    /// <summary>
    /// 键值对
    /// </summary>
    public class KeyValue
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string value { set; get; }
    }
}