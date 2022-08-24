using System;

namespace SA.IOSNative.UIKit {
	public class NativeReceiver : SA.Common.Pattern.Singleton<NativeReceiver>  {



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


		void CalendarPickerClosed(string time) {
			IOSNative.UIKit.Calendar.DatePicked (time);
		}
			
		void DateTimePickerClosed(string time) {
			IOSNative.UIKit.DateTimePicker.PickerClosed(time);
		}

		void DateTimePickerDateChanged(string time) {
			IOSNative.UIKit.DateTimePicker.DateChangedEvent(time);
		}
	}
}

