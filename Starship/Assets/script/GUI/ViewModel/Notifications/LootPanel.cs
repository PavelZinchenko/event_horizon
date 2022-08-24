using System.Collections.Generic;
using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using Services.Gui;
using Services.ObjectPool;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class LootPanel : MonoBehaviour
	{
	    [Inject] private readonly GameObjectFactory _factory;
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public LayoutGroup ItemsGroup;
		public float Cooldown;
		public int MaxItems;

		public void Initialize(WindowArgs args)
		{
		    var items = new List<IProduct>();
            for (var i = 0; i < args.Count; ++i)
		        items.Add(args.Get<IProduct>(i));

			ItemsGroup.InitializeElements<Exploration.ExtractedItem, IProduct>(items, UpdateItem, _factory);
		}
		
		private void UpdateItem(Exploration.ExtractedItem item, IProduct product)
		{
			item.Initialize(product, _resourceLocator);
		}
	}
}
