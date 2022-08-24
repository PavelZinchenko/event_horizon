using UnityEngine;
using UnityEngine.UI;

namespace ViewModel.Skills
{
    public class InformationPanel : MonoBehaviour
    {
        [SerializeField] Text _nameText;
		[SerializeField] Text _descriptionText;
        [SerializeField] Button _unlockButton;
		[SerializeField] GameObject _lockedPanel;
		[SerializeField] GameObject _unlockedPanel;

		public void Cleanup()
		{
			if (_defaultText != null)
				_nameText.text = _defaultText;
			_descriptionText.gameObject.SetActive(false);
			_unlockButton.gameObject.SetActive(true);
			_unlockButton.interactable = false;
			_unlockedPanel.gameObject.SetActive(false);
			_lockedPanel.gameObject.SetActive(false);
		}

		public void Initialize(SkillTreeNode node, bool available, bool unlocked)
        {
            if (_defaultText == null)
                _defaultText = _nameText.text;
			
			_descriptionText.gameObject.SetActive(true);
			_nameText.text = node.Name;
			_descriptionText.text = node.Description;
			_unlockButton.gameObject.SetActive(!unlocked && available);
			_unlockButton.interactable = available;
			_lockedPanel.gameObject.SetActive(!unlocked && !available);
			_unlockedPanel.gameObject.SetActive(unlocked);
        }

        private string _defaultText;
    }
}
