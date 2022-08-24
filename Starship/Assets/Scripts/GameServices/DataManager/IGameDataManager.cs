using Services.Storage;

namespace GameServices.GameManager
{
    public interface IGameDataManager
    {
        void RestorePurchases();
        void CreateNewGame();

        void LoadMod(string id = null);

        void SaveGameToCloud(string filename);
        void SaveGameToCloud(ISavedGame game);
        void LoadGameFromCloud(ISavedGame game);

        void LoadGameFromLocalCopy();
    }
}
