using UnityEngine;
using System.Collections;

public class ISN_RemoteNotificationsRegistrationResult : SA.Common.Models.Result {


	private ISN_DeviceToken _Token;



	public ISN_RemoteNotificationsRegistrationResult(ISN_DeviceToken token) {
		_Token = token;
	}

	public ISN_RemoteNotificationsRegistrationResult(SA.Common.Models.Error error):base(error) {
		
	}




	public ISN_DeviceToken Token {
		get {
			return _Token;
		}
	}
}
