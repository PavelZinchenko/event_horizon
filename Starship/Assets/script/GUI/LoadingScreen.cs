using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameDatabase;
using GameDatabase.Extensions;
using GameServices.LevelManager;
using Services.Localization;
using Services.Reources;
using Zenject;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Text _shipNameText;
    [SerializeField] private Image _shipSprite;
    [SerializeField] private Image _shipIcon;
    [SerializeField] private Image _background;

    [Inject] private readonly IResourceLocator _resourceLocator;
    [Inject] private readonly IDatabase _database;

    [Inject]
    private void Initialize(SceneBeforeUnloadSignal sceneBeforeUnloadSignal, SceneLoadedSignal sceneLoadedSignal, ILocalization localization)
    {
        _sceneBeforeUnloadSignal = sceneBeforeUnloadSignal;
        _sceneLoadedSignal = sceneLoadedSignal;
        _localization = localization;
        _sceneBeforeUnloadSignal.Event += OnSceneUnloaded;
        _sceneLoadedSignal.Event += OnSceneLoaded;
    }

    private void OnSceneUnloaded()
    {
        _canvas.enabled = true;

        if (_firstTime)
        {
            _shipIcon.gameObject.SetActive(false);
            _background.gameObject.SetActive(false);
            _firstTime = false;
        }
        else
        {
            _shipIcon.gameObject.SetActive(true);
            _background.gameObject.SetActive(true);
            var ship = _database.ShipBuildList.Available().NormalShips().RandomUniqueElements(1, _random).First().Ship;
            _shipNameText.text = _localization.GetString(ship.Name);
            _shipIcon.sprite = _resourceLocator.GetSprite(ship.IconImage) ?? _resourceLocator.GetSprite(ship.ModelImage);
            _shipSprite.sprite = _resourceLocator.GetSprite(ship.ModelImage);
        }
    }

    private void OnSceneLoaded()
    {
        _canvas.enabled = false;
    }

    private bool _firstTime = true;
    private readonly System.Random _random = new System.Random();
    private SceneBeforeUnloadSignal _sceneBeforeUnloadSignal;
    private SceneLoadedSignal _sceneLoadedSignal;
    private ILocalization _localization;
}
