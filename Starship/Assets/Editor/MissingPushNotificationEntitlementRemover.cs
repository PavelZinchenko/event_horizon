// C# example:	
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Text.RegularExpressions;

public class MissingPushNotificationEntitlementRemover {


	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		if (target != BuildTarget.iOS) {
			return;
		}

		#if UNITY_5_5_OR_NEWER
			UnityPost550Remover.remove(pathToBuiltProject);
		#else
			UnityPre550Remover.remove(pathToBuiltProject);
		#endif
	}

	private static class UnityPre550Remover {
		public static string appControllerFile = "UnityAppController.mm";
		// this matches (void)application:did..something..Notification..something... methods declaration
		private static string regexpForNotificationMethods = "-\\s?\\(void\\)application:\\(UIApplication\\s?\\*\\)application\\sdid.+RemoteNotification.+\\n?{[^-|#.+]+";

		public static void remove(string pathToBuiltProject) {
			Debug.Log("Running Push Notification Entitlement Warning Remover...");

			// check if app controller file exists
			string classesDirectory = Path.Combine(pathToBuiltProject, "Classes");
			string pathToAppController = Path.Combine(classesDirectory, appControllerFile);

			bool fileExists = File.Exists(pathToAppController);

			if (!fileExists) {
				Debug.Log("App Controller file doesn't exist.");
				return;
			}

			string code = File.ReadAllText(pathToAppController);
			string codeWithDeletedNotificationsMethod = Regex.Replace(code, regexpForNotificationMethods, "");

			File.WriteAllText(pathToAppController, codeWithDeletedNotificationsMethod);
			Debug.Log("Push Notification Entitlement Warning Remover Completed");
		}
	}

	private static class UnityPost550Remover {
		public static string preprocessorHeaderFile = "Preprocessor.h";

		private static string regexpForNotificationMethods = "define UNITY_USES_REMOTE_NOTIFICATIONS 1";

		public static void remove(string pathToBuiltProject) {

			Debug.Log("Running Push Notification Entitlement Warning Remover...");

			// check if app controller file exists
			string classesDirectory = Path.Combine(pathToBuiltProject, "Classes");
			string pathToPreprocessorHeader = Path.Combine(classesDirectory, preprocessorHeaderFile);

			bool fileExists = File.Exists(pathToPreprocessorHeader);

			if (!fileExists) {
				Debug.Log("Preprocessor file doesn't exist.");
				return;
			}

			string code = File.ReadAllText(pathToPreprocessorHeader);
			string codeWithUnsetdNotificationsDeclaration = Regex.Replace(code, regexpForNotificationMethods, "define UNITY_USES_REMOTE_NOTIFICATIONS 0");

			File.WriteAllText(pathToPreprocessorHeader, codeWithUnsetdNotificationsDeclaration);
			Debug.Log("Push Notification Entitlement Warning Remover Completed");
		}
	}
		
}
