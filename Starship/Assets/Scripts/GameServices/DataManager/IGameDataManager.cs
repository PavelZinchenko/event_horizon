using Services.Storage;

namespace GameServices.GameManager
{
    public interface IGameDataManager
    {
        void CreateNewGame();

        void LoadMod(string id = null, bool force = false);
        void ReloadMod();

        void SaveGameToCloud(string filename);
        void SaveGameToCloud(ISavedGame game);
        void LoadGameFromCloud(ISavedGame game);

        void LoadGameFromLocalCopy();
    }
}
