using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;

namespace Services.Storage
{
    public class AndroidLocalStorage : LocalStorage
    {
        public AndroidLocalStorage()
        {
            _internalDirectory = GetInternalStoragePath() + "/";
            _externalDirectory = GetExternalStoragePath() + "/";
            OptimizedDebug.Log("AndroidLocalStorage: internal dir - " + _internalDirectory);
            OptimizedDebug.Log("AndroidLocalStorage: external dir - " + _externalDirectory);
        }

        protected override bool TryLoadMainFile(out byte[] data)
        {
            _preferExternalStorage = false;
            var internalFileName = _internalDirectory + MainFileName;
            var externalFileName = _externalDirectory + MainFileName;

            if (GetFileTime(internalFileName) >= GetFileTime(externalFileName) && TryLoadFile(internalFileName, out data))
                return true;

            if (!TryLoadFile(externalFileName, out data))
                return false;

            _preferExternalStorage = true;
            return true;
        }

        protected override IEnumerable<byte[]> LoadAllBackupFiles()
        {
            foreach (var name in BackupFileNames)
            {
                OptimizedDebug.Log("Loading backup file: " + name);

                byte[] data;
                if (TryLoadFile(name, out data))
                    yield return data;
            }
        }

        protected override bool TrySaveMainFile(byte[] data)
        {
            TryCreateDirectory(_internalDirectory);
            TryCreateDirectory(_externalDirectory);
            var internalFileSaved = TrySaveFile(_internalDirectory + MainFileName, data);
            var externalFileSaved = TrySaveFile(_externalDirectory + MainFileName, data);
            if (internalFileSaved || externalFileSaved)
                return true;

            return TrySaveFile(DefaultSavesDir + MainFileName, data);
        }

        protected override bool TryBackupMainFile()
        {
            return _preferExternalStorage ? 
                TryCopyFile(_externalDirectory + MainFileName, _externalDirectory + BackupFileName) :
                TryCopyFile(_internalDirectory + MainFileName, _internalDirectory + BackupFileName);
        }

        protected override bool TryStoreErrorFile(byte[] data)
        {
            return TrySaveFile(_externalDirectory + ErrorFileName, data);
        }

        private IEnumerable<string> BackupFileNames
        {
            get
            {
                if (_preferExternalStorage)
                    yield return _internalDirectory + MainFileName;
                else
                    yield return _externalDirectory + MainFileName;

                var path1 = _internalDirectory + BackupFileName;
                var path2 = _externalDirectory + BackupFileName;

                if (GetFileTime(path2) > GetFileTime(path1))
                {
                    yield return path2;
                    yield return path1;
                }
                else
                {
                    yield return path1;
                    yield return path2;
                }

                yield return DefaultSavesDir + MainFileName;
                yield return DefaultSavesDir + BackupFileName;
            }
        }

        private static long GetFileTime(string path)
        {
            try
            {
                return File.GetLastWriteTimeUtc(path).Ticks;
            }
            catch (Exception)
            {
                return DateTime.MinValue.Ticks;
            }
        }

        protected override bool TryBackupOldVersion(byte[] data, string version)
        {
            var filename = MainFileName + "." + version;
            return TrySaveFile(_externalDirectory + filename, data);
        }

        private static string GetInternalStoragePath()
        {
            try
            {
                var unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
                var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                var file = context.Call<AndroidJavaObject>("getFilesDir");
                return file.Call<string>("getAbsolutePath");
            }
            catch (System.Exception e)
            {
                OptimizedDebug.LogException(e);
            }

            return string.Empty;
        }

        private static string GetExternalStoragePath()
        {
            try
            {
                var unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
                var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                var file = context.Call<AndroidJavaObject>("getExternalFilesDir", (string)null);
                return file.Call<string>("getAbsolutePath");
            }
            catch (System.Exception e)
            {
                OptimizedDebug.LogException(e);
            }

            return string.Empty;
        }

        private bool _preferExternalStorage;
        private readonly string _externalDirectory;
        private readonly string _internalDirectory;
    }
}
