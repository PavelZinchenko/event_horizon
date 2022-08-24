using GameModel.GameData;

namespace GameModel.Level
{
    public interface ILevelManager
    {
        void Exit();
        void LoadMainMenu();
        void LoadConstructor();
        void LoadControlsSetup();
        void LoadGalaxyMap();
        void LoadCombat();
        void LoadExploration();
        void LoadSkillTree();
        void ReloadLevel();
        bool IsLoading { get; }
        string Name { get; }
    }

    public static class LevelNames
    {
        public const string MainMenu       = "MainMenu";
        public const string Constructor    = "Constructor";
        public const string Controls       = "Controls";
        public const string GalaxyMap      = "GalaxyMap";
        public const string Combat         = "Combat";
        public const string Exploration    = "Exploration";
        public const string SkillTree      = "SkillTree";
    }
}
