using Domain.Quests;
using UnityEngine;
using Services.Gui;
using ViewModel.Quests;

namespace Gui.Quests
{
    public class QuestEventDialog : MonoBehaviour
    {
        [SerializeField] private DescriptionPanel _description;
        [SerializeField] private FleetPanel _fleet;
        [SerializeField] private ItemsPanel _items;
        [SerializeField] private ActionsPanel _actions;

        public void Initialize(WindowArgs args)
        {
            var data = args.Get<IUserInteraction>();
            _description.Initialize(data.Message, data.CharacterName, data.CharacterAvatar);
            _actions.Initialize(data.Actions);
            if (_fleet) _fleet.Initialize(data.Enemies);
            if (_items) _items.Initialize(data.Loot);
        }
    }
}
