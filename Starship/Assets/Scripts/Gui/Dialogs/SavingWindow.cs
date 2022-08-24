using Services.Gui;
using Services.Storage;
using UnityEngine;
using Zenject;

namespace Gui.Common
{
    public class SavingWindow : MonoBehaviour
    {
        [Inject]
        private void Initialize(CloudStorageStatusChangedSignal cloudStorageStatusChangedSignal)
        {
            _cloudStorageStatusChangedSignal = cloudStorageStatusChangedSignal;
            _cloudStorageStatusChangedSignal.Event += OnCloudStorageStatusChanged;
        }

        private void OnCloudStorageStatusChanged(CloudStorageStatus status)
        {
            var window = GetComponent<IWindow>();

            if (status == CloudStorageStatus.Saving)
                window.Open();
            else
                window.Close();
        }

        private CloudStorageStatusChangedSignal _cloudStorageStatusChangedSignal;
    }
}
