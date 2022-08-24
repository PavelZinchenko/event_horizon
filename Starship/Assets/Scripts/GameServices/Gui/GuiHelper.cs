using System;
using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameServices.Quests;
using Services.Gui;
using Zenject;

namespace GameServices.Gui
{
    public class GuiHelper
    {
        [Inject] private readonly IGuiManager _guiManager;
        [Inject] private readonly ShowMessageSignal.Trigger _showMessageTrigger;

#if !EDITOR_MODE
        [Inject] private readonly ItemTypeFactory _itemFactory;
        [Inject] private readonly IQuestManager _questManager;
#endif

        public void ShowItemInfoWindow(IProduct item)
        {
            _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.ItemInfoWindow, new WindowArgs(item));
        }

#if !EDITOR_MODE
        public void ShowItemInfoWindow(IShip ship)
        {
            var item = new Product(_itemFactory.CreateShipItem(ship));
            _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.ItemInfoWindow, new WindowArgs(item));
        }
#else
        public void ShowItemInfoWindow(IShip ship)
        {
            throw new NotImplementedException();
        }
#endif

        public void ShowLootWindow(IEnumerable<IProduct> items)
        {
            _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.LootWindow, new WindowArgs(items.Cast<object>().ToArray()));
        }

        public void ShowConfirmation(string text, System.Action action)
        {
            _guiManager.OpenWindow(global::Gui.Common.WindowNames.ConfirmationDialog, new WindowArgs(text), result =>
            {
                if (result == WindowExitCode.Ok)
                    action.Invoke();
            });
        }

        public void ShowConfirmation(string text, Price price, System.Action action)
        {
            _guiManager.OpenWindow(global::Gui.Common.WindowNames.BuyConfirmationDialog, new WindowArgs(text, price), result =>
            {
                if (result == WindowExitCode.Ok)
                    action.Invoke();
            });
        }

        public void ShowMessage(string message)
        {
            _showMessageTrigger.Fire(message);
        }
    }
}
