////////////////////////////////////////////////////////////////////////////////
//  
// @module Stan's Assets Commons Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace SA.Common.Editor {


	public static class VersionsManager  {




		//private const string UM_IOS_INSTALATION_MARK = SA.Common.Config.IOS_DESTANATION_PATH + "UM_IOS_INSTALATION_MARK.txt";


		//--------------------------------------
		// Android Native
		//--------------------------------------

        public static bool Is_SpotifyInstalled {
            get {
                return SA.Common.Util.Files.IsFileExists(SA.Common.Config.SPOTIFY_VERSION_INFO_PATH);
            }
        }

        public static int Spotify_Version
        {
            get
            {
                return GetVersionCode(SA.Common.Config.SPOTIFY_VERSION_INFO_PATH);
            }
        }

        public static int Spotify_MagorVersion
        {
            get
            {
                return GetMagorVersionCode(SA.Common.Config.SPOTIFY_VERSION_INFO_PATH);
            }
        }

        public static string Spotify_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.SPOTIFY_VERSION_INFO_PATH);
			}
		}

        public static bool Is_AN_Installed {
			get { 
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.AN_VERSION_INFO_PATH);
			}
		}

		public static int AN_Version {
			get {
				return GetVersionCode(SA.Common.Config.AN_VERSION_INFO_PATH);
			}
		}

		public static int AN_MagorVersion {
			get {
				return GetMagorVersionCode(SA.Common.Config.AN_VERSION_INFO_PATH);
			}
		}


		public static string AN_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.AN_VERSION_INFO_PATH);
			}
		}

		//--------------------------------------
		// Mobile Social 
		//--------------------------------------


		public static bool Is_MSP_Installed {
			get {
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.MSP_VERSION_INFO_PATH);
			}
		}

		public static int MSP_Version {

			get {
				return GetVersionCode(SA.Common.Config.MSP_VERSION_INFO_PATH);
			}
		}

		public static int MSP_MagorVersion {

			get {
				return GetMagorVersionCode(SA.Common.Config.MSP_VERSION_INFO_PATH);
			}
		}

		public static string MSP_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.MSP_VERSION_INFO_PATH);
			}
		}

		//--------------------------------------
		// Ultimate Mobile  
		//--------------------------------------

		public static bool Is_UM_Installed {
			get {
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.UM_VERSION_INFO_PATH);
			} 
		}

		public static int UM_Version {
			get {
				return GetVersionCode(SA.Common.Config.UM_VERSION_INFO_PATH);
			}
		}

		public static int UM_MagorVersion {
			get {
				return GetMagorVersionCode(SA.Common.Config.UM_VERSION_INFO_PATH);
			}
		}

		public static string UM_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.UM_VERSION_INFO_PATH);
			}
		}


		//--------------------------------------
		// Google Mobile Ad  
		//--------------------------------------

		public static bool Is_GMA_Installed {
			get {
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.GMA_VERSION_INFO_PATH);
			} 
		}

		public static int GMA_Version {
			get {
				return GetVersionCode(SA.Common.Config.GMA_VERSION_INFO_PATH);
			} 
		}

		public static int GMA_MagorVersion {
			get {
				return GetMagorVersionCode(SA.Common.Config.GMA_VERSION_INFO_PATH);
			} 
		}

		public static string GMA_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.GMA_VERSION_INFO_PATH);
			}
		}
		//--------------------------------------
		// Mobile Native Pop Up 
		//--------------------------------------
		public static bool Is_MNP_Installed {
			get {
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.MNP_VERSION_INFO_PATH);
			} 
		}

		public static int MNP_Version {
			get {
				return GetVersionCode(SA.Common.Config.MNP_VERSION_INFO_PATH);
			} 
		}

		public static int MNP_MagorVersion {
			get {
				return GetMagorVersionCode(SA.Common.Config.MNP_VERSION_INFO_PATH);
			} 
		}

		public static string MNP_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.MNP_VERSION_INFO_PATH);
			}
		}

		//--------------------------------------
		// IOS Native   
		//--------------------------------------

		public static bool Is_ISN_Installed {
			get {
				return SA.Common.Util.Files.IsFileExists(SA.Common.Config.ISN_VERSION_INFO_PATH);
			} 
		}

		public static int ISN_Version {
			get {
				return GetVersionCode(SA.Common.Config.ISN_VERSION_INFO_PATH);
			} 
		}

		public static int ISN_MagorVersion {
			get {
				return GetMagorVersionCode(SA.Common.Config.ISN_VERSION_INFO_PATH);
			} 
		}


		public static string ISN_StringVersionId {
			get {
				return GetStringVersionId(SA.Common.Config.ISN_VERSION_INFO_PATH);
			}
		}


		//--------------------------------------
		// Utilities
		//--------------------------------------

		public static int ParceMagorVersion(string stringVersionId) {
			string[] versions = stringVersionId.Split (new char[] {'.', '/'});
			int intVersion = Int32.Parse(versions[0]) * 100;
			return  intVersion;
		} 


		private static int GetMagorVersionCode(string versionFilePath) {
			string stringVersionId = SA.Common.Util.Files.Read (versionFilePath);
			return ParceMagorVersion(stringVersionId);
		}



		public static int ParceVersion(string stringVersionId) {
			string[] versions = stringVersionId.Split (new char[] {'.', '/'});
			int intVersion = Int32.Parse(versions[0]) * 100 + Int32.Parse(versions[1]) * 10;
			return  intVersion;
		} 



		private static int GetVersionCode(string versionFilePath) {
			string stringVersionId = SA.Common.Util.Files.Read (versionFilePath);
			return ParceVersion(stringVersionId);
		}



		private static string GetStringVersionId(string versionFilePath) {
			if(SA.Common.Util.Files.IsFileExists(versionFilePath)) {
				return SA.Common.Util.Files.Read (versionFilePath);
			} else {
				return "0.0";
			}
		}


		public static string InstalledPluginsList {

			get {


				string allPluginsInstalled = "";



				if(SA.Common.Util.Files.IsFolderExists(SA.Common.Config.BUNDLES_PATH)) {
					string[] bundles = SA.Common.Util.Files.GetFoldersAt (SA.Common.Config.BUNDLES_PATH);
					foreach(string pluginPath in bundles) {
						string pluginName = System.IO.Path.GetFileName (pluginPath);
						allPluginsInstalled = allPluginsInstalled + " (" + pluginName + ")" + "\n";
					}
				}

				if(SA.Common.Util.Files.IsFolderExists(SA.Common.Config.MODULS_PATH)) {

					string[] modules = SA.Common.Util.Files.GetFoldersAt (SA.Common.Config.MODULS_PATH);
					foreach(string pluginPath in modules) {
						string pluginName = System.IO.Path.GetFileName (pluginPath);
						allPluginsInstalled = allPluginsInstalled + " (" + pluginName + ")" + "\n";
					}
				}




				return allPluginsInstalled;
			}
		}



	}

}

#endif
