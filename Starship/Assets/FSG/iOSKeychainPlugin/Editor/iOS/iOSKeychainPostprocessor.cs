#if UNITY_EDITOR && !UNITY_2017
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FSG.iOSKeychain.Xcode
{
	public class iOSKeychainPostprocessor
	{
		static readonly string[] sourceFiles = new[]
		{
			"KeyChainPlugin.h",
			"KeyChainPlugin.mm",
			"UICKeyChainStore.h",
			"UICKeyChainStore.mm"
		};

		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
		{
#if UNITY_5
			if (buildTarget == BuildTarget.iOS)
#else
			if (buildTarget == BuildTarget.iOS)
#endif
			{
				var projPath = PBXProject.GetPBXProjectPath(buildPath);
				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));
				var targetGuid = proj.TargetGuidByName("Unity-iPhone");

				var instance = ScriptableObject.CreateInstance<KeychainPluginPath>();
				var pluginPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(instance)));
				ScriptableObject.DestroyImmediate(instance);

				var targetPath = pluginPath.Replace("Assets", "Libraries");
				Directory.CreateDirectory(Path.Combine(buildPath, targetPath));

				foreach (var fileName in sourceFiles)
				{
					var sourcePath = Path.Combine(pluginPath, fileName);
					var targetFile = Path.Combine(targetPath, fileName);
					File.Copy(sourcePath, Path.Combine(buildPath, targetFile), true);
					proj.AddFileToBuild(targetGuid, proj.AddFile(targetFile, targetFile, PBXSourceTree.Source));
				}

				File.WriteAllText(projPath, proj.WriteToString());
			}
		}
	}
}
#endif