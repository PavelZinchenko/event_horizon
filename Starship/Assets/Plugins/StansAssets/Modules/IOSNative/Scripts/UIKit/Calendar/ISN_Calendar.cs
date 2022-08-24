using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_IPHONE && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace SA.IOSNative.UIKit {

	internal static class Calendar  {

		static Calendar() {
			NativeReceiver.Instance.Init ();
		}

		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern void _ISN_PickDate(int startYear);
		#endif

		private static Action<DateTime> OnCalendarClosed;

		public static void PickDate( Action<DateTime> callback, int startFromYear = 0) {
			OnCalendarClosed = callback;
				
			#if UNITY_IPHONE && !UNITY_EDITOR
			_ISN_PickDate (startFromYear);
			#endif
		}

		internal static void DatePicked(string time) {
			DateTime dt  = DateTime.Parse(time);
			Calendar.OnCalendarClosed (dt);
		}
	}

}
