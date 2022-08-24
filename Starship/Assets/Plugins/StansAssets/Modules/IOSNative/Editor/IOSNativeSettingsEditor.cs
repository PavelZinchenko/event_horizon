using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using SA.IOSNative.StoreKit;

[CustomEditor(typeof(IOSNativeSettings))]
public class IOSNativeSettingsEditor : Editor
{


    private static GUIContent AppleIdLabel = new GUIContent("Apple Id [?]:", "Your Application Apple ID.");
    private static GUIContent SdkVersion = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");



    private static GUIContent SKPVDLabel = new GUIContent("Store Products View [?]:", "The SKStoreProductViewController class makes it possible to integrate purchasing from Apple’s iTunes, App and iBooks stores directly into iOS 6 applications with minimal coding work.");
    private static GUIContent CheckInternetLabel = new GUIContent("Check Internet Connection[?]:", "If set to true, the Internet connection will be checked before sending load request. Requests will be sent automatically if network becomes available.");
    private static GUIContent PromotedPurchaseSupporLabel = new GUIContent("Promoted Purchase Suppor[?]:", "Your app will support IOS11 promoted purchases.");

    private static GUIContent SendBillingFakeActions = new GUIContent("Enable Editor Testing[?]:", "Fake connect and purchase events will be fired in the editor, can be useful for testing your implementation in Editor.");

    private static GUIContent UseGCCaching = new GUIContent("Use Requests Caching[?]:", "Requests to Game Center will be cached if no Internet connection is available. Requests will be resent on the next Game Center connect event.");

    private static GUIContent AutoLoadSmallImagesLoadTitle = new GUIContent("Autoload Small Player Photo[?]:", "As soon as player info received, small player photo will be requested automatically");
    private static GUIContent AutoLoadBigmagesLoadTitle = new GUIContent("Autoload Big Player Photo[?]:", "As soon as player info received, big player photo will be requested automatically");



    public const string ISN_SCRIPTS = SA.Common.Config.MODULS_PATH + "IOSNative/Scripts/";


    private static string ISN_RemoteNotificationsController_Path = ISN_SCRIPTS + "Messaging/Notifications/Controllers/ISN_RemoteNotificationsController.cs";

    private static string GameCenterManager_Path = ISN_SCRIPTS + "GameCenter/Controllers/GameCenterManager.cs";
    private static string GameCenter_TBM_Path = ISN_SCRIPTS + "GameCenter/Controllers/GameCenter_TBM.cs";
    private static string GameCenter_RTM_Path = ISN_SCRIPTS + "GameCenter/Controllers/GameCenter_RTM.cs";
    private static string ISN_GameSaves_Path = ISN_SCRIPTS + "GameCenter/Controllers/ISN_GameSaves.cs";

    private static string IOSNativeMarketBridge_Path = ISN_SCRIPTS + "StoreKit/Controllers/ISN_BillingNativeBridge.cs";
    private static string IOSStoreProductView_Path = ISN_SCRIPTS + "StoreKit/Models/ISN_StoreProductView.cs";
    private static string SK_StoreReviewController_Path = ISN_SCRIPTS + "StoreKit/Services/Review/SK_StoreReviewController.cs";

    private static string ISN_Security_Path = ISN_SCRIPTS + "System/Security/ISN_Security.cs";

    private static string IOSSocialManager_Path = ISN_SCRIPTS + "Social/Controllers/IOSSocialManager.cs";

    private static string IOSCamera_Path = ISN_SCRIPTS + "Media/Camera/Controllers/IOSCamera.cs";
    private static string ISN_FilePicker_Path = ISN_SCRIPTS + "Media/Gallery/Controllers/ISN_FilePicker.cs";

    private static string IOSVideoManager_Path = ISN_SCRIPTS + "Media/Video/Controllers/IOSVideoManager.cs";
    private static string ISN_ReplayKit_Path = ISN_SCRIPTS + "Media/Video/Controllers/ISN_ReplayKit.cs";

    private static string ISN_MediaController_Path = ISN_SCRIPTS + "Media/MediaPlayer/Controllers/ISN_MediaController.cs";

    private static string ISN_UserNotificationsNativeReceiver_Path = ISN_SCRIPTS + "Messaging/UserNotifications/Controllers/NativeReceiver.cs";
    private static string ISN_UserNotificationsNotificationCenter_Path = ISN_SCRIPTS + "Messaging/UserNotifications/Controllers/NotificationCenter.cs";

    private static string ISN_CloudKit_Path = ISN_SCRIPTS + "iCloud/Controllers/ISN_CloudKit.cs";
    private static string ISN_GestureRecognizer_Path = ISN_SCRIPTS + "System/Gestures/ISN_GestureRecognizer.cs";
    private static string ISN_ForceTouch_Path = ISN_SCRIPTS + "System/Gestures/ISN_ForceTouch.cs";

    private static string ISN_ContactStore_Path = ISN_SCRIPTS + "System/Contacts/ISN_ContactStore.cs";


    private static string ISN_AppController_Path = ISN_SCRIPTS + "Core/ISN_AppController.cs";
    private static string ISN_AppController_Native_Path = SA.Common.Config.IOS_DESTANATION_PATH + "ISN_AppController.mm";

    private static string ISN_Soomla_Path = SA.Common.Config.MODULS_PATH + "IOSNative/Addons/Soomla/Controllers/ISN_SoomlaGrow.cs";


    private static string ISN_PermissionsManager_Path = ISN_SCRIPTS + "System/Privacy/Permissions/ISN_PermissionsManager.cs";



    private GUIStyle _ImageBoxStyle = null;


    void Awake() {
#if !UNITY_WEBPLAYER
        UpdateDefines();
#endif
    }



    private Texture[] _ToolbarImages = null;

    public Texture[] ToolbarImages {
        get {
            if (_ToolbarImages == null) {


                Texture2D appstore = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/appstore.png");
                Texture2D gamecenter = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/gamecenter.png");
                Texture2D media = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/media.png");
                Texture2D settings = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/settings.png");
                Texture2D ios = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/ios.png");
                Texture2D camera = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/camera.png");
                Texture2D editorTesting = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/editorTesting.png");
                Texture2D xcode = SA.Common.Editor.Tools.GetEditorTexture("Plugins/StansAssets/Modules/IOSNative/Editor/Icons/xcode.png");


                List<Texture2D> textures = new List<Texture2D>();
                textures.Add(ios);
                textures.Add(appstore);
                textures.Add(gamecenter);
                textures.Add(camera);
                textures.Add(media);
                //textures.Add(sharing);

                textures.Add(xcode);
                textures.Add(editorTesting);
                textures.Add(settings);


                _ToolbarImages = textures.ToArray();

            }
            return _ToolbarImages;
        }
    }

    private int _Width = 500;
    public int Width {
        get {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect scale = GUILayoutUtility.GetLastRect();

            if (scale.width != 1) {
                _Width = System.Convert.ToInt32(scale.width);
            }

            return _Width;
        }
    }




    public override void OnInspectorGUI() {


#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing IOS Native Settings not available with web player platfo​rm. Please switch to any other platform under Build Settings menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To IOS Platfo​rm",  GUILayout.Width(150))) {
			
#if UNITY_5
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
#else
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
#endif
			
			
			
		}
		EditorGUILayout.EndHorizontal();
		
		if(Application.isEditor) {
			return;
		}
		
#endif



        GUI.changed = false;


        InstallOptions();





        GUI.SetNextControlName("toolbar");
        //Settings.ToolbarIndex = GUILayout.Toolbar(Settings.ToolbarIndex, ToolbarImages, new GUILayoutOption[]{ GUILayout.Height(35f)});

        GUILayoutOption[] toolbarSize = new GUILayoutOption[] { GUILayout.Width(Width - 12), GUILayout.Height(35) };
        Settings.ToolbarIndex = GUILayout.Toolbar(Settings.ToolbarIndex, ToolbarImages, toolbarSize);




        switch (Settings.ToolbarIndex) {
            case 0:
                APISettings();
                EditorGUILayout.Space();
                MoreActions();
                EditorGUILayout.Space();
                AboutGUI();
                break;
            case 1:
                BillingSettings();
                break;
            case 2:
                GameCenterSettings();
                break;
            case 3:
                CameraAndGallery();
                break;
            case 4:
                ReplayKitSetings();
                MediaPlayerSettings();
                break;

            case 5:
                DeploySettings();
                break;
            case 6:
                EditorTesting();
                break;

            case 7:
                ISNSettings();
                ThirdPartySettings();
                break;
        }


        if (GUI.changed) {
            DirtyEditor();
        }


    }

    private void EditorTesting() {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Testing Inside Unity Editor", MessageType.None);


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Store Kit", EditorStyles.boldLabel);
        Settings.InAppsEditorTesting = SA.Common.Editor.Tools.ToggleFiled("Editor Testing", Settings.InAppsEditorTesting);

        GUI.enabled = true;
    }


    private static void DeploySettings() {

        //URL Types

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("URL Types", MessageType.None);
        bool addurlTypeItem = GUILayout.Button("+", GUILayout.Width(20));
        if (addurlTypeItem) {
            SA.IOSNative.Models.UrlType newUlr = new SA.IOSNative.Models.UrlType(SA.Common.Editor.Tools.ApplicationIdentifier);
            newUlr.AddSchemes("url_sheme");
            Settings.UrlTypes.Add(newUlr);
        }

        EditorGUILayout.EndHorizontal();

        foreach (SA.IOSNative.Models.UrlType ulr in Settings.UrlTypes) {
            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUI.indentLevel++;
            {

                EditorGUILayout.BeginHorizontal();

                ulr.IsOpen = EditorGUILayout.Foldout(ulr.IsOpen, ulr.Identifier);
                bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons(ulr, Settings.UrlTypes);
                if (ItemWasRemoved) {
                    return;
                }

                EditorGUILayout.EndHorizontal();


                if (ulr.IsOpen) {



                    EditorGUI.indentLevel++;
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Identifier");
                        ulr.Identifier = EditorGUILayout.TextField(ulr.Identifier);
                        EditorGUILayout.EndHorizontal();



                        EditorGUI.indentLevel++;
                        {
                            for (int i = 0; i < ulr.Schemes.Count; i++) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Scheme " + i.ToString());
                                ulr.Schemes[i] = EditorGUILayout.TextField(ulr.Schemes[i]);

                                bool plus = GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20));
                                if (plus) {
                                    ulr.AddSchemes("url_sheme");
                                }

                                bool rem = GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(20));
                                if (rem) {
                                    ulr.Schemes.Remove(ulr.Schemes[i]);
                                    return;
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.indentLevel--;




                    }
                    EditorGUI.indentLevel--;
                }


            }
            EditorGUI.indentLevel--;


            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();


        //Allowed schemes to query

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Allowed schemes to query", MessageType.None);
        bool addSchemesItem = GUILayout.Button("+", GUILayout.Width(20));
        if (addSchemesItem) {
            SA.IOSNative.Models.UrlType newUlr = new SA.IOSNative.Models.UrlType("url_sheme");
            Settings.ApplicationQueriesSchemes.Add(newUlr);
        }
        EditorGUILayout.EndHorizontal();

        foreach (SA.IOSNative.Models.UrlType scheme in Settings.ApplicationQueriesSchemes) {
            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUI.indentLevel++;
            {

                EditorGUILayout.BeginHorizontal();

                scheme.IsOpen = EditorGUILayout.Foldout(scheme.IsOpen, scheme.Identifier);
                bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons(scheme, Settings.ApplicationQueriesSchemes);
                if (ItemWasRemoved) {
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (scheme.IsOpen) {
                    EditorGUI.indentLevel++;
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Identifier");
                        scheme.Identifier = EditorGUILayout.TextField(scheme.Identifier);
                        EditorGUILayout.EndHorizontal();

                    }
                    EditorGUI.indentLevel--;
                }


            }
            EditorGUI.indentLevel--;


            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();


        //Force Touch Items

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Force Touch Menu Items", MessageType.None);
        bool addFTItem = GUILayout.Button("+", GUILayout.Width(20));
        if (addFTItem) {
            SA.IOSNative.Gestures.ForceTouchMenuItem newItem = new SA.IOSNative.Gestures.ForceTouchMenuItem();
            Settings.ForceTouchMenu.Add(newItem);
        }
        EditorGUILayout.EndHorizontal();

        foreach (SA.IOSNative.Gestures.ForceTouchMenuItem item in Settings.ForceTouchMenu) {
            EditorGUILayout.BeginVertical(GUI.skin.box);


            EditorGUI.indentLevel++;
            {

                EditorGUILayout.BeginHorizontal();

                item.IsOpen = EditorGUILayout.Foldout(item.IsOpen, item.Title);
                bool ItemWasRemoved = SA.Common.Editor.Tools.SrotingButtons(item, Settings.ForceTouchMenu);
                if (ItemWasRemoved) {
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (item.IsOpen) {
                    EditorGUI.indentLevel++;
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Title");
                        item.Title = EditorGUILayout.TextField(item.Title);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Subtitle");
                        item.Subtitle = EditorGUILayout.TextField(item.Subtitle);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Action");
                        item.Action = EditorGUILayout.TextField(item.Action);
                        EditorGUILayout.EndHorizontal();

                    }
                    EditorGUI.indentLevel--;
                }


            }
            EditorGUI.indentLevel--;


            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

       

        EditorGUI.BeginChangeCheck();
        {
            SA.IOSDeploy.SettingsEditor.BuildSettings();
            SA.IOSDeploy.SettingsEditor.LanguageValues();
            SA.IOSDeploy.SettingsEditor.PlistValues();

        }
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(SA.IOSDeploy.ISD_Settings.Instance);
        }





    }

    private void InstallOptions() {

        if (!IsInstalled) {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.HelpBox("Install Required ", MessageType.Error);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            Color c = GUI.color;
            GUI.color = Color.cyan;
            if (GUILayout.Button("Install Plugin", GUILayout.Width(350))) {
                ISN_Plugin_Install();
            }
            EditorGUILayout.Space();
            GUI.color = c;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
        }

        if (IsInstalled) {
            if (!IsUpToDate) {

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.HelpBox("Update Required \nResources version: " + SA.Common.Editor.VersionsManager.ISN_StringVersionId + " Plugin version: " + IOSNativeSettings.VERSION_NUMBER, MessageType.Warning);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                Color c = GUI.color;
                GUI.color = Color.cyan;

                if (CurrentMagorVersion != SA.Common.Editor.VersionsManager.ISN_MagorVersion) {
                    if (GUILayout.Button("How to update", GUILayout.Width(350))) {
                        Application.OpenURL("https://goo.gl/GsUcA0");
                    }
                } else {
                    if (GUILayout.Button("Upgrade Resources", GUILayout.Width(350))) {
                        ISN_Plugin_Install();
                    }
                }

                GUI.color = c;
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

            } else {
                EditorGUILayout.HelpBox("IOS Native Plugin v" + IOSNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);

            }
        }


    }


    public static void APISettings() {

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("(Required) Application Data", MessageType.None);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(AppleIdLabel);
        Settings.AppleId = EditorGUILayout.TextField(Settings.AppleId);
        if (Settings.AppleId.Length > 0) {
            Settings.AppleId = Settings.AppleId.Trim();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("(Optional) Services Settings", MessageType.None);


        //IOSNativeSettings.
        Settings.ExpandModulesSettings = EditorGUILayout.Foldout(Settings.ExpandModulesSettings, "IOS Native Libs");
        if (Settings.ExpandModulesSettings) {
            EditorGUI.indentLevel++;


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.Toggle("ISN Basic Features", true);
            GUI.enabled = true;

            Settings.EnableGameCenterAPI = EditorGUILayout.Toggle("Game Center", Settings.EnableGameCenterAPI);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            Settings.EnableInAppsAPI = EditorGUILayout.Toggle("In-App Purchases", Settings.EnableInAppsAPI);
            Settings.EnableSocialSharingAPI = EditorGUILayout.Toggle("Social Sharing", Settings.EnableSocialSharingAPI);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            Settings.EnableCameraAPI = EditorGUILayout.Toggle("Camera And Gallery", Settings.EnableCameraAPI);

            GUI.enabled = Settings.EnableCameraAPI;

            EditorGUI.BeginChangeCheck();

            Settings.EnablePickerAPI = EditorGUILayout.Toggle("Multy Image Picker", Settings.EnablePickerAPI);

            if (EditorGUI.EndChangeCheck()) {
                int index = EditorUtility.DisplayDialogComplex("Picker API Requires Additional components", "See the documentation, to findout how to instal picker API", "Open Documentation", "Cancel", "Later");
                switch (index) {
                    case 1:
                        Settings.EnablePickerAPI = false;
                        break;

                    case 0:
                        Application.OpenURL("https://goo.gl/XdzXDB");
                        break;
                }
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.BeginHorizontal();
            Settings.EnableMediaPlayerAPI = EditorGUILayout.Toggle("Media Player", Settings.EnableMediaPlayerAPI);
            Settings.EnablePushNotificationsAPI = EditorGUILayout.Toggle("Push Notifications", Settings.EnablePushNotificationsAPI);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Settings.EnableReplayKit = EditorGUILayout.Toggle("Replay Kit ", Settings.EnableReplayKit);
            Settings.EnableCloudKit = EditorGUILayout.Toggle("Cloud Kit ", Settings.EnableCloudKit);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Settings.EnableGestureAPI = EditorGUILayout.Toggle("Gestures API", Settings.EnableGestureAPI);
            Settings.EnableForceTouchAPI = EditorGUILayout.Toggle("Force Touch API", Settings.EnableForceTouchAPI);
            if (Settings.EnableForceTouchAPI) {
                Settings.EnableAppEventsAPI = true;
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            Settings.EnableContactsAPI = EditorGUILayout.Toggle("Contacts API", Settings.EnableContactsAPI);
            Settings.EnableUserNotificationsAPI = EditorGUILayout.Toggle("User Notifications API", Settings.EnableUserNotificationsAPI);
            if (Settings.EnableUserNotificationsAPI) {
                Settings.EnableAppEventsAPI = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Settings.EnablePermissionAPI = EditorGUILayout.Toggle("Privay Permissions API", Settings.EnablePermissionAPI);
            Settings.EnableAppEventsAPI = EditorGUILayout.Toggle("App Events API", Settings.EnableAppEventsAPI);
            EditorGUILayout.EndHorizontal();





            if (EditorGUI.EndChangeCheck()) {
                DirtyEditor();
                UpdateDefines();
            }


            EditorGUI.indentLevel--;
        }
    }




    public static void CameraAndGallery(bool showTitle = true) {


        if (showTitle) {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Gallery ", MessageType.None);
        }



        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Loaded Image Size");
        Settings.MaxImageLoadSize = EditorGUILayout.IntField(Settings.MaxImageLoadSize);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Loaded Image Format");
        Settings.GalleryImageFormat = (IOSGalleryLoadImageFormat)EditorGUILayout.EnumPopup(Settings.GalleryImageFormat);
        EditorGUILayout.EndHorizontal();


        if (Settings.GalleryImageFormat == IOSGalleryLoadImageFormat.JPEG) {
            GUI.enabled = true;
        } else {
            GUI.enabled = false;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("JPEG Compression Rate");
        Settings.JPegCompressionRate = EditorGUILayout.Slider(Settings.JPegCompressionRate, 0f, 1f);
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;


        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Starting from IOS 10.0 you need to describe, why you want user to allow accses to the photo library or camera. The system shows the purpose string when asking the user to allow access", MessageType.Info);

        GUI.enabled = Settings.EnableCameraAPI;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gallery Usage Description");
        Settings.PhotoLibraryUsageDescription = EditorGUILayout.TextField(Settings.PhotoLibraryUsageDescription);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Camera Usage Description");
        Settings.CameraUsageDescription = EditorGUILayout.TextField(Settings.CameraUsageDescription);
        EditorGUILayout.EndHorizontal();


        GUI.enabled = Settings.EnableMediaPlayerAPI;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Apple Music Usage Description");
        Settings.AppleMusicUsageDescription = EditorGUILayout.TextField(Settings.AppleMusicUsageDescription);
        EditorGUILayout.EndHorizontal();



        GUI.enabled = Settings.EnableContactsAPI;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Contacts Usage Description");
        Settings.ContactsUsageDescription = EditorGUILayout.TextField(Settings.ContactsUsageDescription);
        EditorGUILayout.EndHorizontal();

    }



    GUIContent L_IdDLabel = new GUIContent("Leaderboard ID[?]:", "A unique alphanumeric identifier that you create for this leaderboard. Can also contain periods and underscores.");
    GUIContent L_DisplayNameLabel = new GUIContent("Display Name[?]:", "The display name of the leaderboard.");
    GUIContent L_DescriptionLabel = new GUIContent("Description[?]:", "The description of your leaderboard.");

    GUIContent A_IdDLabel = new GUIContent("Achievement ID[?]:", "A unique alphanumeric identifier that you create for this achievement. Can also contain periods and underscores.");
    GUIContent A_DisplayNameLabel = new GUIContent("Display Name[?]:", "The display name of the achievement.");
    GUIContent A_DescriptionLabel = new GUIContent("Description[?]:", "The description of your achievement.");

    private void GameCenterSettings() {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Leaderboards Info", MessageType.None);


        EditorGUI.indentLevel++;
        {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            Settings.ShowLeaderboards = EditorGUILayout.Foldout(Settings.ShowLeaderboards, "Leaderboards");



            EditorGUILayout.EndHorizontal();


            if (Settings.ShowLeaderboards) {
                EditorGUILayout.Space();

                foreach (GK_Leaderboard leaderboard in Settings.Leaderboards) {


                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.BeginHorizontal();



                    if (leaderboard.Info.Texture != null) {
                        GUILayout.Box(leaderboard.Info.Texture, ImageBoxStyle, new GUILayoutOption[] { GUILayout.Width(18), GUILayout.Height(18) });
                    }

                    leaderboard.IsOpen = EditorGUILayout.Foldout(leaderboard.IsOpen, leaderboard.Info.Title);



                    bool ItemWasRemoved = DrawSrotingButtons((object)leaderboard, Settings.Leaderboards);
                    if (ItemWasRemoved) {
                        return;
                    }


                    EditorGUILayout.EndHorizontal();

                    if (leaderboard.IsOpen) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(L_IdDLabel);
                        leaderboard.Info.Identifier = EditorGUILayout.TextField(leaderboard.Info.Identifier);
                        if (leaderboard.Info.Identifier.Length > 0) {
                            leaderboard.Info.Identifier = leaderboard.Info.Identifier.Trim();
                        }
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(L_DisplayNameLabel);
                        leaderboard.Info.Title = EditorGUILayout.TextField(leaderboard.Info.Title);
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.Space();
                        EditorGUILayout.Space();


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(L_DescriptionLabel);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        leaderboard.Info.Description = EditorGUILayout.TextArea(leaderboard.Info.Description, new GUILayoutOption[] { GUILayout.Height(60), GUILayout.Width(200) });
                        leaderboard.Info.Texture = (Texture2D)EditorGUILayout.ObjectField("", leaderboard.Info.Texture, typeof(Texture2D), false);
                        EditorGUILayout.EndHorizontal();

                    }

                    EditorGUILayout.EndVertical();

                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
                    GK_Leaderboard leaderbaord = new GK_Leaderboard("");
                    Settings.Leaderboards.Add(leaderbaord);
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

        }
        EditorGUI.indentLevel--;



        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Achievements Info", MessageType.None);

        EditorGUI.indentLevel++;
        {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            Settings.ShowAchievementsParams = EditorGUILayout.Foldout(Settings.ShowAchievementsParams, "Achievements");



            EditorGUILayout.EndHorizontal();


            if (Settings.ShowAchievementsParams) {
                EditorGUILayout.Space();

                foreach (GK_AchievementTemplate achievement in Settings.Achievements) {


                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.BeginHorizontal();



                    if (achievement.Texture != null) {
                        GUILayout.Box(achievement.Texture, ImageBoxStyle, new GUILayoutOption[] { GUILayout.Width(18), GUILayout.Height(18) });
                    }

                    achievement.IsOpen = EditorGUILayout.Foldout(achievement.IsOpen, achievement.Title);



                    bool ItemWasRemoved = DrawSrotingButtons((object)achievement, Settings.Achievements);
                    if (ItemWasRemoved) {
                        return;
                    }


                    EditorGUILayout.EndHorizontal();

                    if (achievement.IsOpen) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(A_IdDLabel);
                        achievement.Id = EditorGUILayout.TextField(achievement.Id);
                        if (achievement.Id.Length > 0) {
                            achievement.Id = achievement.Id.Trim();
                        }
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(A_DisplayNameLabel);
                        achievement.Title = EditorGUILayout.TextField(achievement.Title);
                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.Space();
                        EditorGUILayout.Space();


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(A_DescriptionLabel);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        achievement.Description = EditorGUILayout.TextArea(achievement.Description, new GUILayoutOption[] { GUILayout.Height(60), GUILayout.Width(200) });
                        achievement.Texture = (Texture2D)EditorGUILayout.ObjectField("", achievement.Texture, typeof(Texture2D), false);
                        EditorGUILayout.EndHorizontal();

                    }

                    EditorGUILayout.EndVertical();

                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
                    GK_AchievementTemplate achievement = new GK_AchievementTemplate();
                    Settings.Achievements.Add(achievement);
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();




        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Game Center API Settings", MessageType.None);


        Settings.AutoLoadUsersBigImages = SA.Common.Editor.Tools.YesNoFiled(AutoLoadBigmagesLoadTitle, Settings.AutoLoadUsersBigImages);
        Settings.AutoLoadUsersSmallImages = SA.Common.Editor.Tools.YesNoFiled(AutoLoadSmallImagesLoadTitle, Settings.AutoLoadUsersSmallImages);


        Settings.UseGCRequestCaching = SA.Common.Editor.Tools.YesNoFiled(UseGCCaching, Settings.UseGCRequestCaching);
        Settings.UsePPForAchievements = SA.Common.Editor.Tools.YesNoFiled("Store Achievements Progress in PlayerPrefs[?]", Settings.UsePPForAchievements);

    }

    private void MediaPlayerSettings() {

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Medial Player ", MessageType.None);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Usage Description");
        Settings.AppleMusicUsageDescription = EditorGUILayout.TextField(Settings.AppleMusicUsageDescription);
        EditorGUILayout.EndHorizontal();
    }

    private void ReplayKitSetings() {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Replay Kit ", MessageType.None);



        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Full screen video edit preview on iPad");

        bool windowed = Settings.RPK_iPadViewType == 0;
        windowed = EditorGUILayout.Toggle(windowed);
        if (windowed) {
            Settings.RPK_iPadViewType = 0;
        } else {
            Settings.RPK_iPadViewType = 1;
        }
        EditorGUILayout.EndHorizontal();



    }


    public static void ThirdPartySettings(bool showTitle = true) {


        if (showTitle) {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Third Party Settings", MessageType.None);

        }

        EditorGUILayout.LabelField("One Signal", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        {

            EditorGUI.BeginChangeCheck();

            Settings.OneSignalEnabled = SA.Common.Editor.Tools.ToggleFiled("Enable One Signal", Settings.OneSignalEnabled);
            if (EditorGUI.EndChangeCheck()) {

                if (Settings.OneSignalEnabled) {
                    if (!(SA.Common.Util.Files.IsFolderExists("Plugins/OneSignal") || SA.Common.Util.Files.IsFolderExists("OneSignal"))) {
                        bool res = EditorUtility.DisplayDialog("One Signal not found", "IOS Native wasn't able to find One Signal libraries in your project. Would you like to download and install it?", "Download", "No Thanks");
                        if (res) {
                            Application.OpenURL(Settings.OneSignalDocsLink);
                        }
                        Settings.OneSignalEnabled = false;
                    }
                }
            }

        }
        EditorGUI.indentLevel--;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
            Application.OpenURL(Settings.OneSignalDocsLink);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.LabelField("Soomla Configuration", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        {


            EditorGUI.BeginChangeCheck();
            bool prevSoomlaState = Settings.EnableSoomla;
            Settings.EnableSoomla = SA.Common.Editor.Tools.ToggleFiled("Enable GROW", Settings.EnableSoomla);
            if (EditorGUI.EndChangeCheck()) {
                UpdateDefines();
            }

            if (!prevSoomlaState && Settings.EnableSoomla) {
                bool res = EditorUtility.DisplayDialog("Soomla Grow", "Make sure you initialize SoomlaGrow when your games starts: \nISN_SoomlaGrow.Init();", "Documentation", "Got it");
                if (res) {
                    Application.OpenURL(Settings.SoomlaDocsLink);
                }
            }




            GUI.enabled = Settings.EnableSoomla;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Game Key");
            Settings.SoomlaGameKey = EditorGUILayout.TextField(Settings.SoomlaGameKey);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Env Key");
            Settings.SoomlaEnvKey = EditorGUILayout.TextField(Settings.SoomlaEnvKey);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

        }
        EditorGUI.indentLevel--;

    }

    private void ISNSettings() {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("IOS Native Settings", MessageType.None);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Disable IOS Native Logs");
        Settings.DisablePluginLogs = EditorGUILayout.Toggle(Settings.DisablePluginLogs);
        EditorGUILayout.EndHorizontal();
    }


    private void MoreActions() {
        Settings.ExpandMoreActionsMenu = EditorGUILayout.Foldout(Settings.ExpandMoreActionsMenu, "More Actions");
        if (Settings.ExpandMoreActionsMenu) {

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Example Settings", GUILayout.Width(140))) {
                LoadExampleSettings();

            }

            if (GUILayout.Button("Remove IOS Native", GUILayout.Width(140))) {
                SA.Common.Editor.RemoveTool.RemovePlugins();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public static void LoadExampleSettings() {

        //--------------------------------------
        //  InAppProducts
        //--------------------------------------

        Settings.InAppProducts.Clear();

        Product SmallPack = new Product();
        SmallPack.IsOpen = false;
        SmallPack.Id = "your.product.id1.here";
        SmallPack.PriceTier = PriceTier.Tier1;
        SmallPack.DisplayName = "Small Pack";
        SmallPack.Type = ProductType.Consumable;


        Product NonConsumablePack = new Product();
        NonConsumablePack.IsOpen = false;
        NonConsumablePack.Id = "your.product.id2.here";
        NonConsumablePack.PriceTier = PriceTier.Tier2;
        NonConsumablePack.DisplayName = "Non Consumable Pack";
        NonConsumablePack.Type = ProductType.Consumable;

        Settings.InAppProducts.Add(SmallPack);
        Settings.InAppProducts.Add(NonConsumablePack);

        //--------------------------------------
        //  GameCenter
        //--------------------------------------

        Settings.Leaderboards.Clear();
        GK_Leaderboard Leaderboard1 = new GK_Leaderboard("your.ios.leaderbord1.id");
        Leaderboard1.IsOpen = false;
        Leaderboard1.Info.Title = "Leaderboard 1";

        Settings.Leaderboards.Clear();
        GK_Leaderboard Leaderboard2 = new GK_Leaderboard("your.ios.leaderbord2.id");
        Leaderboard2.IsOpen = false;
        Leaderboard2.Info.Title = "Leaderboard 2";

        Settings.Leaderboards.Add(Leaderboard1);
        Settings.Leaderboards.Add(Leaderboard2);


        Settings.Achievements.Clear();
        GK_AchievementTemplate Achievement1 = new GK_AchievementTemplate();
        Achievement1.Id = "your.achievement.id1.here";
        Achievement1.IsOpen = false;
        Achievement1.Title = "Achievement 1";


        GK_AchievementTemplate Achievement2 = new GK_AchievementTemplate();
        Achievement2.Id = "your.achievement.id2.here";
        Achievement2.IsOpen = false;
        Achievement2.Title = "Achievement 2";

        GK_AchievementTemplate Achievement3 = new GK_AchievementTemplate();
        Achievement3.Id = "your.achievement.id3.here";
        Achievement3.IsOpen = false;
        Achievement3.Title = "Achievement 3";

        Settings.Achievements.Add(Achievement1);
        Settings.Achievements.Add(Achievement2);
        Settings.Achievements.Add(Achievement3);


        Settings.SoomlaEnvKey = "3c3df370-ad80-4577-8fe5-ca2c49b2c1b4";
        Settings.SoomlaGameKey = "db24ba61-3aa7-4653-a3f7-9c613cb2c0f3";

        SA.Common.Editor.Tools.ApplicationIdentifier = "com.stansassets.iosnative.dev";

#if UNITY_IOS
        PlayerSettings.iOS.appleDeveloperTeamID = "6KDFZTBV4R";
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
#endif



    }
	
	
	public GUIStyle ImageBoxStyle {
		get {
			if(_ImageBoxStyle == null) {
				_ImageBoxStyle =  new GUIStyle();
				_ImageBoxStyle.padding =  new RectOffset();
				_ImageBoxStyle.margin =  new RectOffset();
				_ImageBoxStyle.border =  new RectOffset();
			}
			
			return _ImageBoxStyle;
		}
	}
	
	GUIContent ProductIdDLabel 		= new GUIContent("ProductId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent IsConsLabel 			= new GUIContent("Is Consumable[?]:", "Is product allowed to be purchased more than once?");
	GUIContent DisplayNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the In-App Purchase that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent DescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
	GUIContent PriceTierLabel 		= new GUIContent("Price Tier[?]:", "The retail price for this In-App Purchase subscription.");
	
	
	private void BillingSettings() {
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("In-App Purchases", MessageType.None);
		
		
		EditorGUI.indentLevel++; {
			
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			Settings.ShowStoreKitProducts = EditorGUILayout.Foldout(Settings.ShowStoreKitProducts, "Products");
			
			
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(Settings.ShowStoreKitProducts) {
				
				foreach(Product product in Settings.InAppProducts) {
					
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					
					
					if(product.Texture != null) {
						GUILayout.Box(product.Texture, ImageBoxStyle, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					product.IsOpen 	= EditorGUILayout.Foldout(product.IsOpen, product.DisplayName);
					
					
					
					
					EditorGUILayout.LabelField("           "+ product.Price + "$");
					bool ItemWasRemoved = DrawSrotingButtons((object) product, Settings.InAppProducts);
					if(ItemWasRemoved) {
						return;
					}
					
					
					EditorGUILayout.EndHorizontal();
					
					if(product.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(ProductIdDLabel);
						product.Id	 	= EditorGUILayout.TextField(product.Id);
						if(product.Id.Length > 0) {
							product.Id 		= product.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DisplayNameLabel);
						product.DisplayName	 	= EditorGUILayout.TextField(product.DisplayName);
						EditorGUILayout.EndHorizontal();
						
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(IsConsLabel);
						product.Type	 	= (ProductType) EditorGUILayout.EnumPopup(product.Type);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(PriceTierLabel);
						EditorGUI.BeginChangeCheck();
						product.PriceTier	 	= (PriceTier) EditorGUILayout.EnumPopup(product.PriceTier);
						if(EditorGUI.EndChangeCheck()) {
							product.UpdatePriceByTier();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						product.Description	 = EditorGUILayout.TextArea(product.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						product.Texture = (Texture2D) EditorGUILayout.ObjectField("", product.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
						
					}
					
					EditorGUILayout.EndVertical();
					
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					Product product =  new Product();
					Settings.InAppProducts.Add(product);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			
			
			
			
			
			
			
			
			EditorGUILayout.EndVertical();
		}EditorGUI.indentLevel--;
		
		
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(SKPVDLabel);
		
		/*****************************************/
		
		if(Settings.DefaultStoreProductsView.Count == 0) {
			EditorGUILayout.HelpBox("No Default Store Products View Added", MessageType.Info);
		}
		
		
		
		int i = 0;
		foreach(string str in Settings.DefaultStoreProductsView) {
			EditorGUILayout.BeginHorizontal();
			Settings.DefaultStoreProductsView[i]	 	= EditorGUILayout.TextField(Settings.DefaultStoreProductsView[i]);
			if(Settings.DefaultStoreProductsView[i].Length > 0) {
				Settings.DefaultStoreProductsView[i]		= Settings.DefaultStoreProductsView[i].Trim();
			}
			
			if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
				Settings.DefaultStoreProductsView.Remove(str);
				break;
			}
			EditorGUILayout.EndHorizontal();
			i++;
		}
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Add",  GUILayout.Width(80))) {
			Settings.DefaultStoreProductsView.Add("");
		}
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.Space();
		
		
		EditorGUILayout.HelpBox("API Settings", MessageType.None);

		Settings.InAppsEditorTesting 			  = SA.Common.Editor.Tools.YesNoFiled (SendBillingFakeActions, Settings.InAppsEditorTesting);
		Settings.CheckInternetBeforeLoadRequest   = SA.Common.Editor.Tools.YesNoFiled (CheckInternetLabel, Settings.CheckInternetBeforeLoadRequest);
		Settings.PromotedPurchaseSupport  = SA.Common.Editor.Tools.YesNoFiled (PromotedPurchaseSupporLabel, Settings.PromotedPurchaseSupport);
		Settings.TransactionsHandlingMode = (TransactionsHandlingMode)SA.Common.Editor.Tools.EnumPopup ("Transactions Handling Mode", Settings.TransactionsHandlingMode);
		

		EditorGUILayout.Space();

	}
	
	
	
	private bool DrawSrotingButtons(object currentObject, IList ObjectsList) {
		
		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 
		
		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}
		
		
		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		
		
		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}
		
		return r;
	}
	
	
	private void AboutGUI() {
		
		
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();


		
		SA.Common.Editor.Tools.SelectableLabelField(SdkVersion,   IOSNativeSettings.VERSION_NUMBER);
		SA.Common.Editor.Tools.SupportMail();
		
		SA.Common.Editor.Tools.DrawSALogo();
		
	}
	
	
	
	private static void DirtyEditor() {
#if UNITY_EDITOR
		EditorUtility.SetDirty(Settings);
#endif
	}



	public static IOSNativeSettings Settings {
		get {
			return IOSNativeSettings.Instance;
		}
	} 
	
	public static bool IsInstalled {
		get {
			
			return SA.Common.Editor.VersionsManager.Is_ISN_Installed;
		}
	}
	
	
	public static bool IsUpToDate {
		get {
			
			if(CurrentVersion == SA.Common.Editor.VersionsManager.ISN_Version) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	public static int CurrentVersion {
		get {
			return SA.Common.Editor.VersionsManager.ParceVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	public static int CurrentMagorVersion {
		get {
			return SA.Common.Editor.VersionsManager.ParceMagorVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	
	public static int Version {
		get {
			return SA.Common.Editor.VersionsManager.ISN_Version;
		}
	}
	
	
	public static void ISN_Plugin_Install() {

		SA.Common.Editor.Instalation.IOS_UpdatePlugin ();
		UpdateVersionInfo();
		UpdateDefines();
	}
	
	public static void UpdateVersionInfo() {
		SA.Common.Util.Files.Write(SA.Common.Config.ISN_VERSION_INFO_PATH, IOSNativeSettings.VERSION_NUMBER);
	}
	
	

	
	public static void UpdateDefines(bool forseUpdate = false) {
		
		
		if((!IsInstalled || !IsUpToDate) && !forseUpdate) {
			return;
		}
		
		
		SA.Common.Editor.Tools.ChnageDefineState(ISN_RemoteNotificationsController_Path, 		"PUSH_ENABLED", Settings.EnablePushNotificationsAPI);
		
		
		SA.Common.Editor.Tools.ChnageDefineState(GameCenterManager_Path, 				"GAME_CENTER_ENABLED", Settings.EnableGameCenterAPI);
		SA.Common.Editor.Tools.ChnageDefineState(GameCenter_TBM_Path, 					"GAME_CENTER_ENABLED", Settings.EnableGameCenterAPI);
		SA.Common.Editor.Tools.ChnageDefineState(GameCenter_RTM_Path, 					"GAME_CENTER_ENABLED", Settings.EnableGameCenterAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_GameSaves_Path, 					"GAME_CENTER_ENABLED", Settings.EnableGameCenterAPI);


		
		SA.Common.Editor.Tools.ChnageDefineState(IOSNativeMarketBridge_Path, 			"INAPP_API_ENABLED", Settings.EnableInAppsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(IOSStoreProductView_Path, 			"INAPP_API_ENABLED", Settings.EnableInAppsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_Security_Path, 					"INAPP_API_ENABLED", Settings.EnableInAppsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(SK_StoreReviewController_Path, 					"INAPP_API_ENABLED", Settings.EnableInAppsAPI);

		
		SA.Common.Editor.Tools.ChnageDefineState(IOSSocialManager_Path, 				"SOCIAL_API", Settings.EnableSocialSharingAPI);
		
		SA.Common.Editor.Tools.ChnageDefineState(IOSCamera_Path, 						"CAMERA_API", Settings.EnableCameraAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_FilePicker_Path, 					"PICKER_API", Settings.EnablePickerAPI);
		
		SA.Common.Editor.Tools.ChnageDefineState(IOSVideoManager_Path, 				"VIDEO_API", Settings.EnableMediaPlayerAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_MediaController_Path, 			"VIDEO_API", Settings.EnableMediaPlayerAPI);
		
		SA.Common.Editor.Tools.ChnageDefineState(ISN_ReplayKit_Path, 					"REPLAY_KIT", Settings.EnableReplayKit);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_CloudKit_Path, 					"CLOUD_KIT", Settings.EnableCloudKit);


		SA.Common.Editor.Tools.ChnageDefineState(ISN_GestureRecognizer_Path, 			"GESTURE_API", Settings.EnableGestureAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_ForceTouch_Path, 			"FORCE_TOUCH_API", Settings.EnableForceTouchAPI);


		SA.Common.Editor.Tools.ChnageDefineState(ISN_ContactStore_Path, 			"CONTACTS_API_ENABLED", Settings.EnableContactsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_UserNotificationsNativeReceiver_Path, 			"USER_NOTIFICATIONS_API", Settings.EnableUserNotificationsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_UserNotificationsNotificationCenter_Path, 			"USER_NOTIFICATIONS_API", Settings.EnableUserNotificationsAPI);

		SA.Common.Editor.Tools.ChnageDefineState(ISN_AppController_Path, 			"APP_CONTROLLER_ENABLED", Settings.EnableAppEventsAPI);
		SA.Common.Editor.Tools.ChnageDefineState(ISN_PermissionsManager_Path, 			"PERMISSIONS_API_ENABLED", Settings.EnablePermissionAPI);



		if(!Settings.EnablePermissionAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_Privacy");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Privacy.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Privacy.mm");
		}


		if(!Settings.EnableUserNotificationsAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_UserNotifications");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_UserNotifications.h.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_UserNotifications.h");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_UserNotifications.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_UserNotifications.mm");
		}
			


		if(!Settings.EnableAppEventsAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_AppController");

		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_AppController.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_AppController.mm");
			SA.Common.Editor.Tools.ChnageDefineState(ISN_AppController_Native_Path, "USER_NOTIFICATIONS_ENABLED", Settings.EnableUserNotificationsAPI);
		}

		if(!Settings.EnablePushNotificationsAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_RemoteNotifications");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_RemoteNotifications.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_RemoteNotifications.mm");
		}


		if(!Settings.EnableGestureAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_GestureRecognizer");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_GestureRecognizer.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_GestureRecognizer.mm");
		}



		if(!Settings.EnableGestureAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_GestureRecognizer");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_GestureRecognizer.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_GestureRecognizer.mm");
		}


		if(!Settings.EnableForceTouchAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_ForceTouch");
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_DFContinuousForceTouchGestureRecognizer");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_ForceTouch.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_ForceTouch.mm");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_DFContinuousForceTouchGestureRecognizer.h.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_DFContinuousForceTouchGestureRecognizer.h");
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_DFContinuousForceTouchGestureRecognizer.m.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_DFContinuousForceTouchGestureRecognizer.m");
		}

		
		if(!Settings.EnableGameCenterAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_GameCenter");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
		}
		
		
		if(!Settings.EnableInAppsAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_InApp");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_InApp.mm");
		}
		

		
		if(!Settings.EnableCameraAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_Camera");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Camera.mm");
		}


		if(!Settings.EnablePickerAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_FilePicker");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_FilePicker.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_FilePicker.mm");
		}
		
		if(!Settings.EnableSocialSharingAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_SocialGate");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
		}
		
		if(!Settings.EnableMediaPlayerAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_Media");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Media.mm");
		}
		
		if(!Settings.EnableReplayKit) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_ReplayKit");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
		}
		
		
		if(!Settings.EnableCloudKit) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_CloudKit");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_CloudKit.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_CloudKit.mm");
		}


		if(!Settings.EnableContactsAPI) {
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_Contacts");
		} else {
			SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Contacts.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Contacts.mm");
		}

		
		
		
		if(!Settings.EnableSoomla) {
			SA.Common.Editor.Tools.ChnageDefineState(ISN_Soomla_Path, 						"SOOMLA", Settings.EnableSoomla);
			SA.Common.Editor.Instalation.RemoveIOSFile("ISN_Soomla");
		} else {
			
			if(SA.Common.Util.Files.IsFileExists("Plugins/IOS/libSoomlaGrowLite.a")) {
				SA.Common.Editor.Tools.ChnageDefineState(ISN_Soomla_Path, 						"SOOMLA", Settings.EnableSoomla);
				SA.Common.Util.Files.CopyFile(SA.Common.Config.IOS_SOURCE_PATH + "ISN_Soomla.mm.txt", 	SA.Common.Config.IOS_DESTANATION_PATH + "ISN_Soomla.mm");
			} else {
				
				
				bool res = EditorUtility.DisplayDialog("Soomla Grow not found", "IOS Native wasn't able to find Soomla Grow libraries in your project. Would you like to download and install it?", "Download", "No Thanks");
				if(res) {
					Application.OpenURL(Settings.SoomlaDownloadLink);
				}
				
				Settings.EnableSoomla = false;
				UpdateDefines();
			}
		}
		
	}
	
	
}

