using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BS.Microservice.Web.Model
{
    public class tblButtonName:BaseModel
    {
        #region Model
		
		private string _buttonname;
		private string _remark;
		
		/// <summary>
		/// 
		/// </summary>
		public string ButtonName
		{
			set{ _buttonname=value;}
			get{return _buttonname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		#endregion Model
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreateOn { get; set; }
        public string CreateBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}