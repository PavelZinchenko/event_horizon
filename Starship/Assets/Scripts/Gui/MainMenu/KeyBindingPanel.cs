using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.MainMenu
{
    public class KeyBindingPanel : MonoBehaviour
    {
        [SerializeField] private ToggleGroup _toggleGroup;

        [SerializeField] private Toggle _toggleLeft;
        [SerializeField] private Toggle _toggleRight;
        [SerializeField] private Toggle _toggleForward;
        [SerializeField] private Toggle _toggleAction1;
        [SerializeField] private Toggle _toggleAction2;
        [SerializeField] private Toggle _toggleAction3;
        [SerializeField] private Toggle _toggleAction4;
        [SerializeField] private Toggle _toggleAction5;
        [SerializeField] private Toggle _toggleAction6;

        [SerializeField] private Text _leftText;
        [SerializeField] private Text _rightText;
        [SerializeField] private Text _forwardText;
        [SerializeField] private Text _actionText1;
        [SerializeField] private Text _actionText2;
        [SerializeField] private Text _actionText3;
        [SerializeField] private Text _actionText4;
        [SerializeField] private Text _actionText5;
        [SerializeField] private Text _actionText6;

        //public void OnEnable()
        //{
        //    _inputManager.Controls.Ship.Disable();
        //    UpdateButtonsText();
        //    _toggleGroup.SetAllTogglesOff(false);
        //}

        //public void OnDisable()
        //{
        //    CancelOperation();
        //    _inputManager.Controls.Ship.Enable();
        //}

        //public void OnButtonClicked(bool selected)
        //{
        //    CancelOperation();

        //    if (!selected) return;

        //    if (_toggleLeft.isOn)
        //        _operation = _inputManager.Controls.Ship.TurnLeft.PerformInteractiveRebinding(0);
        //    else if (_toggleRight.isOn)
        //        _operation = _inputManager.Controls.Ship.TurnRight.PerformInteractiveRebinding(0);
        //    else if (_toggleForward.isOn)
        //        _operation = _inputManager.Controls.Ship.MoveForward.PerformInteractiveRebinding(0);
        //    else if (_toggleAction1.isOn)
        //        _operation = _inputManager.Controls.Ship.Action1.PerformInteractiveRebinding(0);
        //    else if (_toggleAction2.isOn)
        //        _operation = _inputManager.Controls.Ship.Action2.PerformInteractiveRebinding(0);
        //    else if (_toggleAction3.isOn)
        //        _operation = _inputManager.Controls.Ship.Action3.PerformInteractiveRebinding(0);
        //    else if (_toggleAction4.isOn)
        //        _operation = _inputManager.Controls.Ship.Action4.PerformInteractiveRebinding(0);
        //    else if (_toggleAction5.isOn)
        //        _operation = _inputManager.Controls.Ship.Action5.PerformInteractiveRebinding(0);
        //    else if (_toggleAction6.isOn)
        //        _operation = _inputManager.Controls.Ship.Action6.PerformInteractiveRebinding(0);
        //    else
        //        return;

        //    _operation.WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f).OnComplete(ButtonRebindCompleted).Start();
        //}

        //private void CancelOperation()
        //{
        //    if (_operation == null) return;
        //    _operation.Cancel();
        //    _operation.Dispose();
        //    _operation = null;
        //}

        //private void ButtonRebindCompleted(InputActionRebindingExtensions.RebindingOperation operation)
        //{
        //    CancelOperation();
        //    UpdateButtonsText();
        //    _toggleGroup.SetAllTogglesOff();
        //}
        
        //private void UpdateButtonsText()
        //{
        //    _leftText.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.TurnLeft.bindings[0].effectivePath);
        //    _rightText.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.TurnRight.bindings[0].effectivePath);
        //    _forwardText.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.MoveForward.bindings[0].effectivePath);
        //    _actionText1.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action1.bindings[0].effectivePath);
        //    _actionText2.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action2.bindings[0].effectivePath);
        //    _actionText3.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action3.bindings[0].effectivePath);
        //    _actionText4.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action4.bindings[0].effectivePath);
        //    _actionText5.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action5.bindings[0].effectivePath);
        //    _actionText6.text = InputControlPath.ToHumanReadableString(_inputManager.Controls.Ship.Action6.bindings[0].effectivePath);
        //}

        //private InputActionRebindingExtensions.RebindingOperation _operation;
        //public void OnButtonClicked(bool selected) { }
    }
}