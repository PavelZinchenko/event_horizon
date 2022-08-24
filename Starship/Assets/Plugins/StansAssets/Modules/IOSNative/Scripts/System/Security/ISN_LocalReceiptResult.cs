////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ISN_LocalReceiptResult  {
	
	private byte[] _Receipt = null;
	private string _ReceiptString = "";

	public ISN_LocalReceiptResult(string data) {
		if(data.Length > 0) {
			_Receipt = System.Convert.FromBase64String(data);
			_ReceiptString = data;
		}
	}



	public byte[] Receipt {
		get {
			return _Receipt;
		}
	}

	public string ReceiptString {
		get {
			return _ReceiptString;
		}
	}
}
