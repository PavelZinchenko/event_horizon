using Services.Localization;
using Services.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.MainMenu
{
    public class SavedGameItem : MonoBehaviour
    {
        [SerializeField] private Text _name;
        [SerializeField] private Text _playTime;

        public ISavedGame Game { get; private set; }

        public void Initialize(ISavedGame savedgame, ILocalization localization)
        {
            Game = savedgame;

            _name.text = savedgame.Name;
            _playTime.text = savedgame.ModificationTime.ToString();
        }
    }
}
