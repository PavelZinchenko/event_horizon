using UnityEngine;
using GameServices.LevelManager;
using Zenject;

public class MainMenuLoader : MonoBehaviour
{
    [Inject] private readonly ILevelLoader _levelLoader;

	void Start () 
	{
        _levelLoader.LoadLevel(LevelName.MainMenu);
	}
}
