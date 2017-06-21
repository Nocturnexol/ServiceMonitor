using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BS.Microservice.Web.Model
{
    /// <summary>
    /// 分页基础类
    /// </summary>
    public class PageInfo
    {
        private int _CurrentPageIndex = 1;
        /// <summary>
        /// 页数
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _CurrentPageIndex; }
            set { _CurrentPageIndex = Math.Max(1, value); }
        }

        private int _PageSize = 10;
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = Math.Max(10, value); }
        }

        private string _Orderby;
        /// <summary>
        /// 排序
        /// </summary>
        public string Orderby
        {
            get { return _Orderby; }
            set { _Orderby = value; }
        }

        private string _TableName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }

        private string _QueryFields = "*";
        /// <summary>
        /// 查询字段
        /// </summary>
        public string QueryFields
        {
            get { return _QueryFields; }
            set
            {
                _QueryFields = String.IsNullOrWhiteSpace(value) ? "*" : value;
            }
        }

        private string _Where;
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Where
        {
            get { return _Where; }
            set { _Where = value; }
        }


        private string _sidx;
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sidx
        {
            get { return _sidx; }
            set { _sidx = value; }
        }

        private string _sord;
        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sord
        {
            get { return _sord; }
            set { _sord = value; }
        }
    }
}
