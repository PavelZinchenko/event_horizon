using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Constructor
{
    public class SatelliteListItem : MonoBehaviour
    {
        public Image Icon;
        public Text NameText;
        public Text QuantityText;
        public Text SizeText;
        public Text WeaponText;
        public GameObject ButtonsPanel;
        public Text ClickToInstallText;
        public Text CantBeInstalledText;
        public Button InstallOnTheLeftButton;
        public Button InstallOnTheRightButton;

        public ItemId<SatelliteBuild> BuildId;
        public ItemId<Satellite> Id;
    }
}
