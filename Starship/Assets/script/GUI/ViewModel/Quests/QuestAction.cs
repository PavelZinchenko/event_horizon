using Domain.Quests;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Services.Localization;

namespace ViewModel
{
	namespace Quests
	{
		public class QuestAction : MonoBehaviour
		{
			[Inject] ILocalization _localization;
            [Inject] QuestEventSignal.Trigger _questEventTrigger;
			
			[SerializeField] private Button _button;
			[SerializeField] private Text _name;
		    [SerializeField] private Text _description;

            [SerializeField] private Image _background;

			[SerializeField] private Color _colorDanger;
			[SerializeField] private Color _colorWarning;
			[SerializeField] private Color _colorInfo;

			public void Initialize(UserAction action)
			{
				_action = action;
				_name.text = _localization.GetString(action.Text);

			    if (_description)
			    {
			        var requirementsText = action.Requirements.GetDescription(_localization);
                    if (string.IsNullOrEmpty(requirementsText))
			        {
			            _description.gameObject.SetActive(false);
			        }
			        else
			        {
			            _description.gameObject.SetActive(true);
			            _description.text = requirementsText;
			        }
			    }

			    //_button.interactable = action.Enabled;

				if (_background != null)
					_background.color = ColorBySeverity(action.Severity);
			}

			public void OnButtonClicked()
			{
				_action.Invoke(_questEventTrigger);
			}

			private Color ColorBySeverity(Severity severity)
			{
				switch (severity)
				{
				case Severity.Danger:
					return _colorDanger;
				case Severity.Warning:
					return _colorWarning;
				case Severity.Info:
				default:
					return _colorInfo;
				}
			}

			private UserAction _action;
		}
	}
}
