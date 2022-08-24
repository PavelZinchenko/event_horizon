using System;
using Economy.ItemType;
using GameDatabase;
using GameServices.Player;
using GameStateMachine;
using Services.Gui;
using Services.IAP;
using Services.Localization;
using Services.Storage;
using Utils;
using Zenject;

namespace GameServices.Gui
{
    public class NotificationManager
    {
        [Inject] private readonly IGuiManager _guiManager;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IStateMachine _gameStateMachine;

        [Inject]
        public NotificationManager(
            InAppPurchaseFailedSignal inAppPurchaseFailedSignal,
            ShowMessageSignal showMessageSignal,
            ShowDebugMessageSignal debugMessageSignal,
            CloudOperationFailedSignal cloudOperationFailedSignal,
            CloudSavingCompletedSignal cloudSavingCompletedSignal,
            CloudLoadingCompletedSignal cloudLoadingCompletedSignal)
        {
            _inAppPurchaseFailedSignal = inAppPurchaseFailedSignal;
            _inAppPurchaseFailedSignal.Event += OnInAppPurchaseFailed;
            _showMessageSignal = showMessageSignal;
            _showMessageSignal.Event += OnShowMessage;
            _cloudLoadingCompletedSignal = cloudLoadingCompletedSignal;
            _cloudLoadingCompletedSignal.Event += OnCloudGameLoaded;
            _cloudSavingCompletedSignal = cloudSavingCompletedSignal;
            _cloudSavingCompletedSignal.Event += OnCloudGameSaved;
            _cloudOperationFailedSignal = cloudOperationFailedSignal;
            _cloudOperationFailedSignal.Event += OnCloudOperationFailed;
            _debugMessageSingal = debugMessageSignal;
            _debugMessageSingal.Event += OnDebugMessage;
        }

        private void OnInAppPurchaseFailed(string reason)
        {
            try
            {
                if (_gameStateMachine.ActiveState == StateType.Initialization || _gameStateMachine.ActiveState == StateType.MainMenu)
                    return;

                UnityEngine.Debug.Log("NotificationManager.OnInAppPurchaseFailed");
                _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.IapErrorWindow, new WindowArgs(reason));
            }
            catch (ArgumentException)
            {
                UnityEngine.Debug.Log("Iap error window can't be opened");
            }
        }

        private void OnCloudGameLoaded()
        {
            OnShowMessage(_localization.GetString("$CloudGameLoaded"));
        }

        private void OnCloudGameSaved()
        {
            OnShowMessage(_localization.GetString("$CloudGameSaved"));
        }

        private void OnCloudOperationFailed(string error)
        {
            OnShowMessage(_localization.GetString("$CloudOperationFailed", error));
        }

        private void OnShowMessage(string message)
        {
            UnityEngine.Debug.Log("OnShowMessage: " + message);

            _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.MessageWindow, new WindowArgs(message));
        }

        private void OnDebugMessage(string message)
        {
            UnityEngine.Debug.Log("OnDebugMessage: " + message);
            _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.DebugLogWindow, new WindowArgs(message));
        }

        private readonly InAppPurchaseFailedSignal _inAppPurchaseFailedSignal;
        private readonly ShowMessageSignal _showMessageSignal;
        private readonly CloudOperationFailedSignal _cloudOperationFailedSignal;
        private readonly CloudSavingCompletedSignal _cloudSavingCompletedSignal;
        private readonly CloudLoadingCompletedSignal _cloudLoadingCompletedSignal;
        private readonly ShowDebugMessageSignal _debugMessageSingal;
    }

    public class ShowMessageSignal : SmartWeakSignal<string>
    {
        public class Trigger : TriggerBase { }
    }

    public class ShowDebugMessageSignal : SmartWeakSignal<string>
    {
        public class Trigger : TriggerBase { }
    }
}
