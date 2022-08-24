//#define FORCE_TOUCH_API

////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


#if UNITY_IPHONE && !UNITY_EDITOR && FORCE_TOUCH_API
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.Gestures {


	public class ForceInfo {

		private float _Force;
		private float _MaxForce;

		public ForceInfo(float force, float maxForce) {
			_Force = force;
			_MaxForce = maxForce;
		}

		public float Force {
			get {
				return _Force;
			}
		}


		public float MaxForce {
			get {
				return _MaxForce;
			}
		}
			
	}



	public class ForceTouch : SA.Common.Pattern.Singleton<ForceTouch> {



		#if UNITY_IPHONE && !UNITY_EDITOR && FORCE_TOUCH_API

		[DllImport ("__Internal")]
		private static extern void _ISN_FT_SUBSCRIBE();

		[DllImport ("__Internal")]
		private static extern void _ISN_FT_SETUP(float forceTouchDelay, float baseForceTouchPressure, float triggeringForceTouchPressure);


		[DllImport ("__Internal")]
		private static extern string _ISN_FT_AppOpenshortcutItem();

		#endif



		public event Action OnForceTouchStarted = delegate {};
		public event Action OnForceTouchFinished = delegate {};
		public event Action<ForceInfo> OnForceChanged = delegate {};
		public event Action<string> OnAppShortcutClick = delegate {};


		private static bool _IsTouchTrigerred = false;

		//--------------------------------------
		// Initialization
		//--------------------------------------


		void Awake() {
			DontDestroyOnLoad (gameObject);

			#if UNITY_IPHONE && !UNITY_EDITOR && FORCE_TOUCH_API
			_ISN_FT_SUBSCRIBE();
			#endif
		}


		//--------------------------------------
		// Public Methpds
		//--------------------------------------


		public void Setup(float forceTouchDelay, float baseForceTouchPressure, float triggeringForceTouchPressure) {
			#if UNITY_IPHONE && !UNITY_EDITOR && FORCE_TOUCH_API
			_ISN_FT_SETUP(forceTouchDelay, baseForceTouchPressure, triggeringForceTouchPressure);
			#endif
		}


		//--------------------------------------
		// Get / Set
		//--------------------------------------

		public static string AppOpenshortcutItem {
			get {
				#if UNITY_IPHONE && !UNITY_EDITOR && FORCE_TOUCH_API
				return _ISN_FT_AppOpenshortcutItem();
				#else
				return string.Empty;
				#endif
			}
		}


		//--------------------------------------
		// Action Handlers
		//--------------------------------------



		void didStartForce(string array) {
			_IsTouchTrigerred = true;
			OnForceTouchStarted ();
		}

		void didForceChanged(string array) {

			if (!_IsTouchTrigerred) {
				return;
			}

			string[] data = array.Split(SA.Common.Data.Converter.DATA_SPLITTER); 


			float force = Convert.ToSingle (data[0]);
			float maxForce = Convert.ToSingle (data[1]);

			var e = new ForceInfo (force, maxForce);
			OnForceChanged (e);

		}

		void didForceEnded(string array) {

			_IsTouchTrigerred = false;
			OnForceTouchFinished ();
		}


		void performActionForShortcutItem(string action) {
			OnAppShortcutClick (action);
		}
			
		 
	}

}
