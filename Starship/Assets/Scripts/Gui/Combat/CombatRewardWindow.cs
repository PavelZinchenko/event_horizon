using GameModel.Quests;
using UnityEngine;
using Services.Gui;

namespace Gui.Combat
{
    public class CombatRewardWindow : MonoBehaviour
    {
        public ViewModel.Quests.ItemsPanel Items;

        public void Initialize(WindowArgs args)
        {
            var reward = args.Get<IReward>(0);
            Items.Initialize(reward);
        }
    }
}
