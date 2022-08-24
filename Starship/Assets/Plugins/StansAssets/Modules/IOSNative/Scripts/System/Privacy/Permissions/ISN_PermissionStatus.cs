using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.IOSNative.Privacy {
	
	public enum PermissionStatus  {
		
		NotDetermined = 0, //Explicit user permission is required for photo library access, but the user has not yet granted or denied such permission.
		Restricted = 1, //Your app is not authorized to access the photo library, and the user cannot grant such permission.
		Denied = 2, //The user has explicitly denied your app access to the photo library.
		Authorized = 3 //The user has explicitly granted your app access to the photo library.

	}
}
