using System;
using System.IO;
using GameServices.Gui;
using Services.Gui;
using Services.Localization;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.Screenshots
{
    public class SharingManager : ISharingManager
    {
        [Inject] private ILocalization _localization;
        [Inject] private GuiHelper _guiHelper;

        public void ShareScreenshot(Texture2D screenshot, string filename, string pcDirectory)
        {
            string filePath;

#if UNITY_EDITOR
            filePath = Path.Combine(Application.persistentDataPath, pcDirectory, filename);
            OptimizedDebug.Log(filePath);
#elif UNITY_STANDALONE
            filePath = Path.Combine(Application.dataPath, "..", pcDirectory, filename);
#elif UNITY_ANDROID
            filePath = Path.Combine(Application.temporaryCachePath, filename);
#endif
            filePath = Path.GetFullPath(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, screenshot.EncodeToPNG());

#if UNITY_STANDALONE
            var message = _localization.GetString("$ScreenshotSaved", filePath.Replace("\\", "/"));
            _guiHelper.ShowMessage(message);
#elif UNITY_ANDROID
            new NativeShare().AddFile(filePath).SetCallback((result, target) =>
            {
                switch (result)
                {
                    case NativeShare.ShareResult.Shared:
                        _guiHelper.ShowMessage("$ScreenshotShared");
                        break;
                    case NativeShare.ShareResult.NotShared:
                        _guiHelper.ShowMessage("$ScreenshotSharedCanceled");
                        break;
                    case NativeShare.ShareResult.Unknown:
                    default:
                        _guiHelper.ShowMessage("$ScreenshotSharedUnknown");
                        break;
                }
            }).Share();
#endif
        }
    }
}
