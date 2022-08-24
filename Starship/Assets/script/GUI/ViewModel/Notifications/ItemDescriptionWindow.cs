using Economy.Products;
using UnityEngine;
using Services.Gui;
using ViewModel.Craft;

namespace ViewModel
{
    public class ItemDescriptionWindow : MonoBehaviour
    {
        public ItemDescriptionPanel Description;

        public void Initialize(WindowArgs args)
        {
            var item = args.Get<IProduct>(0);

            Description.Initialize(item);
        }
    }
}
