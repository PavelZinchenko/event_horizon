//#define PERMISSIONS_API_ENABLED

using System;
using System.Collections.Generic;

#if UNITY_IPHONE && !UNITY_EDITOR && PERMISSIONS_API_ENABLED
using System.Runtime.InteropServices;
#endif

namespace SA.IOSNative.Privacy {
	
	public static class PermissionsManager {
		private static Dictionary<string, Action<PermissionStatus>> OnResponseDictionary;
	

		static PermissionsManager() {
			OnResponseDictionary = new Dictionary<string, Action<PermissionStatus>>();
		}

		#if UNITY_IPHONE && !UNITY_EDITOR && PERMISSIONS_API_ENABLED
		[DllImport ("__Internal")]
		private static extern int _ISN_CheckPermissions(string descriptionKey);

		[DllImport ("__Internal")]
		private static extern int _ISN_RequestPermissions(string descriptionKey);
		#endif


		//--------------------------------------
		//  Public Methods
		//--------------------------------------

		public static PermissionStatus CheckPermissions(Permission permission) {
			#if UNITY_IPHONE && !UNITY_EDITOR && PERMISSIONS_API_ENABLED
			return (PermissionStatus) _ISN_CheckPermissions (permission.ToString ());
			#else
			return PermissionStatus.NotDetermined;
			#endif

		}

		public static void RequestPermission(Permission permission, Action<PermissionStatus> callback) {
			if (NativeReceiver.Instance == null) {
				NativeReceiver.Instance.Init ();
			}

			OnResponseDictionary[permission.ToString ()] = callback;

			#if UNITY_IPHONE && !UNITY_EDITOR && PERMISSIONS_API_ENABLED
			_ISN_RequestPermissions (permission.ToString ());
			#endif
		}

		internal static void PermissionRequestResponse(string permissionData) {
			string[] DataArray = permissionData.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

			for (int i = 0; i < DataArray.Length; i++) {
				if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
					break;
				}
			}

			if (DataArray.Length > 0) {
				string callbackKey = DataArray [0];

				Action<PermissionStatus> callback = OnResponseDictionary [callbackKey];
				if (callback != null) {
					string permissionStatusString = DataArray [1];
					if (permissionStatusString != null) {
						try {
							int permissionStatusCode = Int32.Parse(permissionStatusString);

							PermissionStatus permissionStatus = (PermissionStatus) permissionStatusCode;
							callback(permissionStatus);
						} catch (FormatException e) {
							ISN_Logger.Log (e.ToString ());
						}

					}

				}
			}
		}
			
	}
}

