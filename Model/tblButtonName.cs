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
    }
}