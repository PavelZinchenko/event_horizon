////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.IO;
using System.Threading;

namespace SA.Common.Util {

	public static class Files {


		public static bool IsFileExists(string fileName) {
			if (fileName.Equals (string.Empty)) {
				return false;
			}
			
			return File.Exists (GetFullPath (fileName));
		}

		public static void CreateFile(string fileName) {
			if(!IsFileExists(fileName)) {
				CreateFolder (fileName.Substring (0, fileName.LastIndexOf ('/')));

				#if UNITY_4 || UNITY_5

				FileStream stream = File.Create (GetFullPath (fileName));
				stream.Close();

				#else
				File.Create (GetFullPath (fileName));
				#endif
			}

		}

		public static void Write(string fileName, string contents) {



			CreateFolder (fileName.Substring (0, fileName.LastIndexOf ('/')));

			TextWriter tw = new StreamWriter(GetFullPath (fileName), false);
			tw.Write(contents);
			tw.Close(); 

			AssetDatabase.Refresh();
			
			//File.WriteAllText(GetFullPath (fileName), contents);
		}

		public static string Read(string fileName) {
			#if !UNITY_WEBPLAYER
			if(IsFileExists(fileName)) {
				return File.ReadAllText(GetFullPath (fileName));
			} else {
				return "";
			}
			#endif

			#if UNITY_WEBPLAYER
			Debug.LogWarning("FileStaticAPI::Read is innored under wep player platfrom");
			return "";
			#endif
		}
		
		public static void CopyFile(string srcFileName, string destFileName) {
			if (IsFileExists (srcFileName) && !srcFileName.Equals(destFileName)) {
				int index = destFileName.LastIndexOf("/");
				string filePath = string.Empty;
				
				if (index != -1) {
					filePath = destFileName.Substring(0, index);
				}
				
				if (!Directory.Exists(GetFullPath(filePath))) {
					Directory.CreateDirectory(GetFullPath(filePath));
				}
				
				File.Copy(GetFullPath(srcFileName), GetFullPath(destFileName), true);
				
				AssetDatabase.Refresh();
			}
		}
		
		public static void DeleteFile(string fileName, bool refresh = true) {
			if (IsFileExists (fileName)) {
				File.Delete(GetFullPath(fileName));

				if (refresh) {
					AssetDatabase.Refresh();
				}
			}
		}
		
		public static bool IsFolderExists(string folderPath) {
			if (folderPath.Equals (string.Empty)) {
				return false;
			}
			
			return Directory.Exists (GetFullPath(folderPath));
		}

		public static string[] GetFoldersAt(string folderPath) {
			
			return Directory.GetDirectories (GetFullPath(folderPath));
		}

		public static void CreateFolder(string folderPath) {
			if (!IsFolderExists (folderPath)) {
				Directory.CreateDirectory (GetFullPath (folderPath));

				AssetDatabase.Refresh();
			}
		}

		public static void CopyFolder(string srcFolderPath, string destFolderPath) {

			#if !UNITY_WEBPLAYER
			if (!IsFolderExists (srcFolderPath)) {
				return;
			}

			CreateFolder(destFolderPath);


			srcFolderPath = GetFullPath (srcFolderPath);
			destFolderPath = GetFullPath (destFolderPath);
			
			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*", SearchOption.AllDirectories)) {
				Directory.CreateDirectory(dirPath.Replace(srcFolderPath, destFolderPath));
			}
			
			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(srcFolderPath, "*.*", SearchOption.AllDirectories)) {

				File.Copy(newPath, newPath.Replace(srcFolderPath, destFolderPath), true);
			}
			
			AssetDatabase.Refresh ();
			#endif

			#if UNITY_WEBPLAYER
			Debug.LogWarning("FileStaticAPI::CopyFolder is innored under wep player platfrom");
			#endif
		}
		
		public static void DeleteFolder(string folderPath, bool refresh = true) {
			#if !UNITY_WEBPLAYER
			if (IsFolderExists (folderPath)) {

				Directory.Delete(GetFullPath(folderPath), true);

				if (refresh) {
					AssetDatabase.Refresh();
				}
			}
			#endif

			#if UNITY_WEBPLAYER
			Debug.LogWarning("FileStaticAPI::DeleteFolder is innored under wep player platfrom");
			#endif
		}
		
		private static string GetFullPath(string srcName) {
			if (srcName.Equals (string.Empty)) {
				return Application.dataPath;
			}
			
			if (srcName [0].Equals ('/')) {
				srcName.Remove(0, 1);
			}
			
			return Application.dataPath + "/" + srcName;
		}


		
		//////////////////////////////////////////////////////////////////////
		/// UPDATE WITH AssetDatabase interface
		//////////////////////////////////////////////////////////////////////	
		
		public static void CreateAssetFolder(string assetFolderPath) {
			if (!IsFolderExists (assetFolderPath)) {
				int index = assetFolderPath.IndexOf("/");
				int offset = 0;
				string parentFolder = "Assets";
				while (index != -1) {
					if (!Directory.Exists(GetFullPath(assetFolderPath.Substring(0, index)))) {
						string guid = AssetDatabase.CreateFolder(parentFolder, assetFolderPath.Substring(offset, index - offset));
						AssetDatabase.GUIDToAssetPath(guid);
					}
					offset = index + 1;
					parentFolder = "Assets/" + assetFolderPath.Substring(0, offset - 1);
					index = assetFolderPath.IndexOf("/", index + 1);
				}
				
				AssetDatabase.Refresh();
			}
		}
		
		public static void CopyAsset(string srcAssetName, string destAssetName) {
			if (IsFileExists (srcAssetName) && !srcAssetName.Equals(destAssetName)) {
				int index = destAssetName.LastIndexOf("/");
				string filePath = string.Empty;
				
				if (index != -1) {
					filePath = destAssetName.Substring(0, index + 1);
					//Create asset folder if needed
					CreateAssetFolder(filePath);
				}

		
				AssetDatabase.CopyAsset(GetFullAssetPath(srcAssetName), GetFullAssetPath(destAssetName));
				AssetDatabase.Refresh();
			}
		}
		
		public static void DeleteAsset(string assetName) {
			if (IsFileExists (assetName)) {
				AssetDatabase.DeleteAsset(GetFullAssetPath(assetName));			
				AssetDatabase.Refresh();
			}
		}
		
		private static string GetFullAssetPath(string assetName) {
			if (assetName.Equals (string.Empty)) {
				return "Assets/";
			}
			
			if (assetName [0].Equals ('/')) {
				assetName.Remove(0, 1);
			}
			
			return "Assets/" + assetName;
		}
		
		//////////////////////////////////////////////////////////////////////
	}

}

#endif
