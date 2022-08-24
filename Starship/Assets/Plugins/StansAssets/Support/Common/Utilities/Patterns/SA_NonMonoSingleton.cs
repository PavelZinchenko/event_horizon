////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

namespace SA.Common.Pattern {

	public abstract class NonMonoSingleton<T>  where T : NonMonoSingleton<T>, new() {

		private static T _Instance = null;
		
		public static T Instance {
			get {
				if (_Instance == null) {
					_Instance = new T();
				}

				return _Instance;
			}

		}
	}
}
