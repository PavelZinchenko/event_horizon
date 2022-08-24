using UnityEngine;

namespace Services.Storage
{
    public interface IDataStorage
    {
        void Save(IGameData data);
        bool TryLoad(IGameData data, string mod);
        bool TryImportOriginalSave(IGameData gameData, string mod);
    }
}
