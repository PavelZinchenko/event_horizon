////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;


namespace SA.Common.Models {

	public class Error  {

		private int _Code = 0;
		private string _Messgae = string.Empty;



		//--------------------------------------
		// Intialization
		//--------------------------------------

		public Error() {
			_Code = 0;
			_Messgae = "Unknown Error";
		}

		public Error(int code, string message = "") {
			_Code = code;
			_Messgae = message;
		}


		public Error(string errorData) {
			string[] data = errorData.Split(SA.Common.Data.Converter.DATA_SPLITTER);

			_Code = System.Convert.ToInt32(data[0]);
			_Messgae = data[1];
		}



		//--------------------------------------
		// Get / Set
		//--------------------------------------


		/// <summary>
		/// Error Code
		/// </summary>
		public int Code {
			get {
				return _Code;
			}
		}


		/// <summary>
		/// Error Describtion Message
		/// </summary>
		public string Message {
			get {
				return _Messgae;
			}
		}

	}


}