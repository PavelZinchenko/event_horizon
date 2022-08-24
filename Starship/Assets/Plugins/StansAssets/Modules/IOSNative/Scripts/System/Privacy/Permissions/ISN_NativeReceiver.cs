using System;

namespace SA.IOSNative.Privacy
{
	public class NativeReceiver : SA.Common.Pattern.Singleton<NativeReceiver>
	{

		//--------------------------------------
		// Initialization
		//--------------------------------------


		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		public void Init() {

		}



		//--------------------------------------
		// Native Events
		//--------------------------------------
		void PermissionRequestResponseReceived(string permissionData) {
			PermissionsManager.PermissionRequestResponse (permissionData);
		}
	}
}

