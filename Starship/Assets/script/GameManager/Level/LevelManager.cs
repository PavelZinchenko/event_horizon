using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameModel.Level
{
    public class LevelManager : MonoBehaviour//, ILevelManager
    {
        //public void Exit()
        //{
        //    if (IsLoading)
        //        return;

        //    if (string.IsNullOrEmpty(_lastLevel) || SceneManager.GetActiveScene().name == _lastLevel)
        //        LoadMainMenu();
        //    else
        //        LoadLevel(_lastLevel, false);
        //}

        //public void LoadMainMenu()
        //{
        //    LoadLevel(LevelNames.MainMenu);
        //}

        //public void LoadConstructor()
        //{
        //    LoadLevel(LevelNames.Constructor);
        //}

        //public void LoadControlsSetup()
        //{
        //    LoadLevel(LevelNames.Controls);
        //}

        //public void LoadGalaxyMap()
        //{
        //    LoadLevel(LevelNames.GalaxyMap);
        //}

        //public void LoadCombat()
        //{
        //    LoadLevel(LevelNames.Combat);
        //}

        //public void LoadExploration()
        //{
        //    LoadLevel(LevelNames.Exploration);
        //}

        //public void LoadSkillTree()
        //{
        //    LoadLevel(LevelNames.SkillTree);
        //}

        //public void ReloadLevel()
        //{
        //    LoadLevel(_currentLevel, true);
        //}

        //public bool IsLoading
        //{
        //    get
        //    {
        //        if (_loadLevelOperation == null)
        //            return false;

        //        if (_loadLevelOperation.isDone)
        //        {
        //            _loadLevelOperation = null;
        //            return false;
        //        }

        //        return true;
        //    }
        //}

        //public string Name { get { return _currentLevel; } }

        //private void LoadLevel(string name, bool force = false)
        //{
        //    if (_currentLevel == name && !force)
        //    {
        //        UnityEngine.Debug.Log("Level already loaded: " + name);
        //        return;
        //    }

        //    //switch (name)
        //    //{
        //    //    case LevelNames.Combat:
        //    //        ServiceLocator.MusicPlayer.PlayCombatMusic();
        //    //        break;
        //    //    case LevelNames.GalaxyMap:
        //    //        ServiceLocator.MusicPlayer.PlayGalaxyMusic();
        //    //        break;
        //    //    case LevelNames.MainMenu:
        //    //        ServiceLocator.MusicPlayer.PlayMenuMusic();
        //    //        break;
        //    //    case LevelNames.Exploration:
        //    //        ServiceLocator.MusicPlayer.PlayPlanetMusic();
        //    //        break;
        //    //}

        //    //UnityEngine.Debug.Log("Loading Level: " + name);
        //    //ServiceLocator.Analytics.LogScene(name);

        //    foreach(var canvas in FindObjectsOfType<Canvas>())
        //        canvas.enabled = false;
            
        //    _loadLevelOperation = SceneManager.LoadSceneAsync(name);

        //    ServiceLocator.Messenger.Broadcast(EventType.LevelBeforeClose);

        //    _lastLevel = SceneManager.GetActiveScene().name;
        //    _currentLevel = name;
        //}

        //private string _currentLevel;
        //private string _lastLevel;
        //private AsyncOperation _loadLevelOperation;
    }
}
