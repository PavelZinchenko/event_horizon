using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Callbacks;

public class ApplicationManager : EditorWindow
{
	[MenuItem("Window/Application Manager")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(ApplicationManager));
	}

	private void OnGUI()
	{
		if (!_initialized) Initialize();

		GUILayout.Label("Actions", EditorStyles.boldLabel);
        if (EditorApplication.isPlaying)
		{
			GUILayout.Label("Cannot modify config in play mode.");
        }
        else
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Update config file"))
				GenerateAppConfigFile(false);

            if (GUILayout.Button("Generate ship prefabs"))
                GenerateShipPrefabs();

            EditorGUILayout.EndHorizontal();
		}
        
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		UpdateId(EditorGUILayout.TextField("Bundle id", PlayerSettings.applicationIdentifier));
		UpdateVersion(EditorGUILayout.TextField("Bundle version", PlayerSettings.bundleVersion));
		UpdateAndroidVersionCode(EditorGUILayout.TextField("Android version code", PlayerSettings.Android.bundleVersionCode.ToString()));

		GUILayout.Label("Plugins", EditorStyles.boldLabel);

		GUILayout.Label("Output", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("Target dir", AssetsDir + ConfigDir);
		EditorGUILayout.LabelField("Config class name", ClassName);
	}

    private void Initialize()
	{
		_initialized = true;
	}

    private void GenerateShipPrefabs()
    {
        var shipSprites = LoadAllAssets<Sprite>("/Sprites/Ships").ToArray();
        var defaultPrefab = Resources.Load<GameObject>("Combat/Ships/Default");

        foreach (var sprite in shipSprites)
        {
            var prefabPath = "Combat/Ships/" + sprite.name;
            var prefab = Resources.Load<GameObject>(prefabPath);

            UnityEngine.Debug.Log(sprite.name + (prefab ? " - ok" : " - not found"));
            
            if (prefab)
                continue;

            var ship = GameObject.Instantiate(defaultPrefab);
            ship.name = sprite.name;
            ship.GetComponent<SpriteRenderer>().sprite = sprite;
            ship.AddComponent<PolygonCollider2D>();

            PrefabUtility.CreatePrefab("Assets/Resources/" + prefabPath + ".prefab", ship);
            GameObject.DestroyImmediate(ship);
        }
    }

    private IEnumerable<T> LoadAllAssets<T>(string path) where T : UnityEngine.Object
    {
        var files =
            Directory.GetFiles(Application.dataPath + path, "*", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith(".meta"));
        foreach (var file in files)
        {
            var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
                yield return asset;
        }
    }

    private void UpdateId(string id)
	{
		if (id == PlayerSettings.applicationIdentifier) return;

		PlayerSettings.applicationIdentifier = id;

		Debug.Log("bundle id updated");
    }

	private void UpdateVersion(string version)
	{
		if (version == PlayerSettings.bundleVersion) return;

        PlayerSettings.bundleVersion = version;

		Debug.Log("bundle version updated");
    }

	private void UpdateAndroidVersionCode(string version)
	{
		try
		{
			var code = System.Convert.ToInt32(version);
			if (code != PlayerSettings.Android.bundleVersionCode)
			{
				PlayerSettings.Android.bundleVersionCode = code;
				Debug.Log("android bundle version code updated");
            }
        }
        catch (System.Exception)
        {
        }
	}

    [PostProcessBuild]
    private static void OnPostBuild(UnityEditor.BuildTarget buildTarget, string pathToBuildProject)
    {
        GenerateAppConfigFile(true);
    }

    private static void GenerateAppConfigFile(bool increaseBuildNumber)
	{
		var code = string.Empty;
		code += "public static class " + ClassName + Environment.NewLine;
		code += "{" + Environment.NewLine;
		code += "    public const string bundleIdentifier = \"" + PlayerSettings.applicationIdentifier + "\";" + Environment.NewLine;
		code += "    public const string version = \"" + PlayerSettings.bundleVersion + "\";" + Environment.NewLine;
        code += "    public const int versionCode = " + PlayerSettings.Android.bundleVersionCode + ";" + Environment.NewLine;
        code += "    public const int buildNumber = " + (increaseBuildNumber ? AppConfig.buildNumber + 1 : AppConfig.buildNumber) + ";" + Environment.NewLine;
        code += "}" + Environment.NewLine;

		CreateNewBuildVersionClassFile(code);
	}

	private static void CreateNewBuildVersionClassFile(string code)
	{
		if (System.String.IsNullOrEmpty (code))
		{
			Debug.Log ("Code generation stopped, no code to write.");
		}

		CheckOrCreateDirectory(AssetsDir + ConfigDir);

		var fileName = AssetsDir + ConfigDir + "/" + ClassName + ".cs";
		bool success = false;
		using (StreamWriter writer = new StreamWriter(fileName, false)) 
		{
			try 
			{
				writer.WriteLine ("{0}", code);
				success = true;
			} 
			catch (System.Exception ex) 
			{
				string msg = " \n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog("Error when trying to generate file " + fileName, msg, "OK");
			}
		}
		if (success) 
		{
			AssetDatabase.Refresh (ImportAssetOptions.Default);
		}
	}
	
	private static void CheckOrCreateDirectory(string dir) 
	{
		if (File.Exists(dir)) 
		{
			Debug.LogWarning(dir + " is a file instead of a directory !");
			return;
		}
		else if (!Directory.Exists(dir)) 
		{
			try 
			{
				Directory.CreateDirectory(dir);
			}
			catch (System.Exception ex) 
			{
				Debug.LogWarning(ex.Message);
				throw ex;
			}
		}
	}

	private bool _initialized = false;
	private const string AssetsDir = "Assets/";
	private const string ConfigDir = "Script/Config/Generated";
	private const string ClassName = "AppConfig";
}
