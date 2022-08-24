using System.Collections.Generic;
using UnityEngine;

namespace Services.Storage
{
    public class MacLocalStorage : LocalStorage
    {
        protected override bool TryLoadMainFile(out byte[] data)
        {
            return TryLoadFile(_savesDir + MainFileName, out data);
        }

        protected override IEnumerable<byte[]> LoadAllBackupFiles()
        {
            foreach (var name in BackupFileNames)
            {
                byte[] data;
                if (TryLoadFile(name, out data))
                    yield return data;
            }
        }

        protected override bool TrySaveMainFile(byte[] data)
        {
            TryCreateDirectory(_savesDir);
            return TrySaveFile(_savesDir + MainFileName, data);
        }

        protected override bool TryBackupMainFile()
        {
            return TryCopyFile(_savesDir + MainFileName, _savesDir + BackupFileName);
        }

        protected override bool TryStoreErrorFile(byte[] data)
        {
            return TrySaveFile(_savesDir + ErrorFileName, data);
        }

        private IEnumerable<string> BackupFileNames
        {
            get
            {
                yield return _savesDir + BackupFileName;
                yield return _oldSavesDir + MainFileName;
                yield return _oldSavesDir + BackupFileName;
            }
        }

        private readonly string _savesDir = Application.dataPath + "/../../SavesDir/";
        private readonly string _oldSavesDir = Application.persistentDataPath + "/";
    }
}
