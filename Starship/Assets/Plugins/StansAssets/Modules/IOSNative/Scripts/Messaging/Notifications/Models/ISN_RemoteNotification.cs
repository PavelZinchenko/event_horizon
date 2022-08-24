using UnityEngine;
using System.Collections;

public class ISN_RemoteNotification  {


	private string _Body = "{}";


	public ISN_RemoteNotification(string body) {
		_Body = body;
	}


	public string Body {
		get {
			return _Body;
		}
	}
}
