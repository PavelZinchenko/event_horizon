////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

namespace SA.IOSNative.UIKit {
	public static class DateTimePicker  {
		
		private static event Action<DateTime> OnPickerClosed;
		private static event Action<DateTime> OnPickerDateChanged;

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		[DllImport ("__Internal")]
		private static extern void _ISN_ShowDP(int mode);

		[DllImport ("__Internal")]
		private static extern void _ISN_ShowDPWithTime(int mode, double seconds);
		#endif


		static DateTimePicker() {
			NativeReceiver.Instance.Init ();
		}

		//--------------------------------------
		// Public Methods
		//--------------------------------------

		/// <summary>
		/// Displays DateTimePickerUI with DateTimePicker Mode.
		///
		///<param name="mode">An object that contains the IOSDateTimePicker mode.</param>
		/// </summary>	
		public static void Show(DateTimePickerMode mode, Action<DateTime> callback) {
			OnPickerClosed = callback;
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowDP( (int) mode);
			#endif
		}

		/// <summary>
		/// Displays DateTimePickerUI with DateTimePicker Mode and pre-set date.
		///
		///<param name="mode">An object that contains the IOSDateTimePicker mode</param>
		///<param name="name">An object DateTime that contains pre-set date</param>
		/// </summary>
		public static void Show(DateTimePickerMode mode, DateTime dateTime, Action<DateTime> callback) {
			OnPickerClosed = callback;

			DateTime sTime = new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc);
			double unixTimestamp = (dateTime - sTime).TotalSeconds;
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowDPWithTime( (int) mode, unixTimestamp);	
			#endif
		}

		//--------------------------------------
		// Events
		//--------------------------------------

		internal static void DateChangedEvent(string time) {
			DateTime dt  = DateTime.Parse(time);
			OnPickerDateChanged(dt);
		}

		internal static void PickerClosed(string time) {
			DateTime dt  = DateTime.Parse(time);
			OnPickerClosed (dt);
		}

	}
}