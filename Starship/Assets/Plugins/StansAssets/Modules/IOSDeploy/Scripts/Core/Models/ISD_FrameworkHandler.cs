using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;



namespace SA.IOSDeploy {
	public class ISD_FrameworkHandler : MonoBehaviour {



		private static List<Framework> _DefaultFrameworks = null;

		public static List<Framework> AvailableFrameworks{
			get{
				List<Framework> resultList = new List<Framework> ();
				List<string> strings = new List<string>( Enum.GetNames (typeof(iOSFramework)));
				foreach (Framework frmwrk in ISD_Settings.Instance.Frameworks) {
					if (strings.Contains(frmwrk.Type.ToString())) {
						strings.Remove (frmwrk.Type.ToString());
					}
				}
				foreach (Framework frmwrk in DefaultFrameworks) {
					if (strings.Contains(frmwrk.Type.ToString())) {
						strings.Remove (frmwrk.Type.ToString());
					}
				}
				foreach (iOSFramework v in Enum.GetValues(typeof(iOSFramework))) {
					if(strings.Contains(v.ToString())){
						resultList.Add(new Framework((iOSFramework)v) );
					}
				}
				return resultList;
			}
		}

		public static List<string> GetImportedFrameworks(){
			List<string> FoundedFrameworks = new List<string> ();
			#if !UNITY_WSA
			DirectoryInfo DirectoryPath = new DirectoryInfo (Application.dataPath);

			string[] dirrExtensions = new[] { ".framework"};
			FileInfo[] allFiles = DirectoryPath.GetFiles ("*", SearchOption.AllDirectories);
			DirectoryInfo[] dirrFiles = DirectoryPath.GetDirectories ("*", SearchOption.AllDirectories);

			allFiles = allFiles.Where(f => dirrExtensions.Contains(f.Extension.ToLower())).ToArray();
			dirrFiles = dirrFiles.Where (f => dirrExtensions.Contains (f.Extension.ToLower ())).ToArray ();

			foreach (FileInfo file in allFiles) {
				string NewFrameworkName = file.Name;
				FoundedFrameworks.Add (NewFrameworkName);
			}


			foreach (DirectoryInfo file in dirrFiles) {
				string NewFrameworkName = file.Name;
				FoundedFrameworks.Add (NewFrameworkName);
			}
			#endif
			return FoundedFrameworks;
		}


		public static List<string> GetImportedLibraries(){
			List<string> FoundedFrameworks = new List<string> ();
			#if !UNITY_WSA
			DirectoryInfo DirectoryPath = new DirectoryInfo (Application.dataPath);

			string[] fileExtensions = new[] {".a", ".dylib"};
			FileInfo[] allFiles = DirectoryPath.GetFiles ("*", SearchOption.AllDirectories);

			allFiles = allFiles.Where(f => fileExtensions.Contains(f.Extension.ToLower())).ToArray();
			foreach (FileInfo file in allFiles) {
				string NewFrameworkName = file.Name;
				FoundedFrameworks.Add (NewFrameworkName);
			}
			#endif
			return FoundedFrameworks;
		}

		public static List<Framework> DefaultFrameworks{
			get{
				if(_DefaultFrameworks == null){
					_DefaultFrameworks = new List<Framework>();
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreText));
					_DefaultFrameworks.Add (new Framework (iOSFramework.AudioToolbox));
					_DefaultFrameworks.Add (new Framework (iOSFramework.AVFoundation));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CFNetwork));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreGraphics));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreLocation));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreMedia));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreMotion));
					_DefaultFrameworks.Add (new Framework (iOSFramework.CoreVideo));
					_DefaultFrameworks.Add (new Framework (iOSFramework.Foundation));
					_DefaultFrameworks.Add (new Framework (iOSFramework.iAd));
					_DefaultFrameworks.Add (new Framework (iOSFramework.MediaPlayer));
					_DefaultFrameworks.Add (new Framework (iOSFramework.OpenAL));
					_DefaultFrameworks.Add (new Framework (iOSFramework.OpenGLES));
					_DefaultFrameworks.Add (new Framework (iOSFramework.QuartzCore));
					_DefaultFrameworks.Add (new Framework (iOSFramework.SystemConfiguration));
					_DefaultFrameworks.Add (new Framework (iOSFramework.UIKit));
				}
				return _DefaultFrameworks;
			}
		}

		public static string[] BaseFrameworksArray(){
			List<string> array = new List<string> (AvailableFrameworks.Capacity);
			foreach (Framework framework in AvailableFrameworks) {
				array.Add (framework.Type.ToString ());
			}
			return array.ToArray ();
		} 

	}
}
