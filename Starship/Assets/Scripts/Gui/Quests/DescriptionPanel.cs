using GameDatabase.Model;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Services.Localization;
using Services.Reources;

namespace Gui.Quests
{
    public class DescriptionPanel : MonoBehaviour
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private Text _messageText;
        [SerializeField] private Text _characterName;
        [SerializeField] private Text _characterMessageText;
        [SerializeField] private Image _characterAvatar;
        [SerializeField] private Image _unknownAvatar;
        [SerializeField] private GameObject _characterPanel;
        [SerializeField] private GameObject _messagePanel;

        public void Initialize(string text, string characterName, SpriteId avatar)
        {
            if (string.IsNullOrEmpty(text))
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (string.IsNullOrEmpty(characterName) || !_characterPanel)
            {
                if (_characterPanel) _characterPanel.gameObject.SetActive(false);
                if (_messagePanel) _messagePanel.gameObject.SetActive(true);
                _messageText.text = _localization.GetString(text);
            }
            else
            {
                _characterPanel.gameObject.SetActive(true);
                _messagePanel.gameObject.SetActive(false);
                _characterMessageText.text = _localization.GetString(text);
                _characterName.text = _localization.GetString(characterName);

                var sprite = _resourceLocator.GetSprite(avatar);
                _characterAvatar.sprite = sprite;
                _characterAvatar.gameObject.SetActive(sprite);
                _unknownAvatar.gameObject.SetActive(!sprite);
            }
        }
    }
}
