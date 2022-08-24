using System.ComponentModel;

namespace GameServices.LevelManager
{
    public enum LevelName
    {
        Undefined,
        CommonGui,
        MainMenu,
        StarMap,
        Battle,
        SkillTree,
        Constructor,
        ConfigureControls,
        Combat,
        Exploration,
        Ehopedia,
    }

    public static class LevelNameExtension
    {
        public static string ToSceneId(this LevelName level)
        {
            switch (level)
            {
                case LevelName.Undefined:
                    return string.Empty;
                case LevelName.CommonGui:
                    return "CommonGuiScene";
                case LevelName.MainMenu:
                    return "MainMenuScene";
                case LevelName.StarMap:
                    return "StarMapScene";
                case LevelName.Battle:
                    return "BattleScene";
                case LevelName.SkillTree:
                    return "SkillTreeScene";
                case LevelName.Constructor:
                    return "ConstructorScene";
                case LevelName.ConfigureControls:
                    return "ConfigureControlsScene";
                case LevelName.Combat:
                    return "CombatScene";
                case LevelName.Exploration:
                    return "ExplorationScene";
                case LevelName.Ehopedia:
                    return "EhopediaScene";
                default:
                    throw new InvalidEnumArgumentException(nameof(level), (int)level, level.GetType());
            }
        }
    }
}
