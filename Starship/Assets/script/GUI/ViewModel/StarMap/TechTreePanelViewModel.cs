using System;
using System.Collections.Generic;
using System.Linq;
using DataModel.Technology;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Database;
using GameServices.Gui;
using GameServices.Research;
using UnityEngine;
using UnityEngine.UI;
using Services.Audio;
using Services.Localization;
using Services.ObjectPool;
using Services.Reources;
using Utils;
using Zenject;

namespace ViewModel
{
	public class TechTreePanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly Research _research;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly ITechnologies _technologies;
	    [Inject] private readonly GameObjectFactory _factory;
        [Inject] private readonly GuiHelper _helper;
	    [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private RectTransform Content;
        [SerializeField] private Button ResearchButton;
        [SerializeField] private Button MoreInfoButton;
        [SerializeField] private ToggleGroup ToggleGroup;
        [SerializeField] private AudioClip ResearchSound;
        [SerializeField] private int Width;
        [SerializeField] private int Height;
        [SerializeField] private int BorderSize;
        [SerializeField] private Color NormalColor;
        [SerializeField] private Color LockedColor;
        [SerializeField] private float MinScale = 0.4f;
	    [SerializeField] private Text _itemNameText;
        [SerializeField] private Text _alreadyResearchedText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private GameObject _itemIconPanel;

        public void Initialize(Faction faction)
        {
            if (_faction == faction)
                return;

            _faction = faction;
            ResearchButton.interactable = false;
            MoreInfoButton.interactable = false;
            _selectedItem = null;
            UpdateTree(faction);
			ToggleGroup.SetAllTogglesOff();
            OnTechDeselected(null);
        }

		public void OnTechSelected(ITechnology technology)
		{
		    var researched = _research.IsTechResearched(technology);
            var available = _research.IsTechAvailable(technology);
			ResearchButton.interactable = !researched && available && !technology.Hidden;
            MoreInfoButton.interactable = available;
            _selectedItem = technology;

		    _itemNameText.text = technology.GetName(_localization);
            _alreadyResearchedText.gameObject.SetActive(researched);
            _itemIconPanel.gameObject.SetActive(true);
		    _itemIcon.sprite = technology.GetImage(_resourceLocator);
		}

		public void OnTechDeselected(ITechnology technology)
		{
			ResearchButton.interactable = false;
            MoreInfoButton.interactable = false;

            _itemNameText.text = string.Empty;
            _alreadyResearchedText.gameObject.SetActive(false);
            _itemIconPanel.gameObject.SetActive(false);
        }

        public void MoreInfoButtonClicked()
	    {
	        if (_selectedItem == null)
                return;

            _helper.ShowItemInfoWindow(_selectedItem.CreateItem());
	    }

		public void OnResearchButtonClicked()
		{
			if (_selectedItem.Hidden || !_research.ResearchTech(_selectedItem))
				return;

			UpdateTree(_selectedItem.Faction);
			_soundPlayer.Play(ResearchSound);
			ResearchButton.interactable = false;
		}

		public void Zoom(float value)
		{
			_zoom = Mathf.Clamp(_zoom/value, MinScale, 1f);
			Content.transform.localScale = Vector3.one*_zoom;
		}

	    private void OnEnable()
	    {
	        Initialize(Faction.Neutral);
	    }

	    private void OnDisable()
	    {
	        _faction = Faction.Undefined;
	    }

		private void UpdateTree(Faction faction)
		{
			var techTree = TechTreeBulder.Build(faction, Width, Height, _technologies, _research);
			var nodes = CreateNodes(techTree);
            var links = GetLinks(nodes);
			UpdateLinks(links);
			UpdateContentSize();
		}

		private void UpdateContentSize()
		{
			float width = 0, height = 0;
			foreach (Transform transform in Content)
			{
				if (!transform.gameObject.activeSelf)
					continue;

				var rectTransform = transform.GetComponent<RectTransform>();
				var offset = rectTransform.anchoredPosition;

				width = Math.Max(width, offset.x + Width/2);
				height = Math.Max(height, offset.y + Height/2);
			}

			Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			Content.transform.localScale = Vector3.one*_zoom;
		}

		private Dictionary<ITechnology, RectTransform> CreateNodes(IEnumerable<KeyValuePair<ITechnology, Vector2>> nodes)
		{
			var enumerator = nodes.GetEnumerator();
			var items = new Dictionary<ITechnology, RectTransform>();
			RectTransform item = null;
			foreach (Transform transform in Content.transform)
			{
				var viewModel = transform.GetComponent<TechItemViewModel>();
				if (viewModel == null)
					continue;
				
				item = viewModel.GetComponent<RectTransform>();
				if (enumerator.MoveNext())
				{
					items.Add(enumerator.Current.Key, item);
                    UpdateTechItem(viewModel, enumerator.Current.Key, enumerator.Current.Value);
                }
				else
				{
					item.gameObject.SetActive(false);
				}
			}
			
			while (enumerator.MoveNext())
			{
				var newItem = _factory.Create(item.gameObject).GetComponent<RectTransform>();
				newItem.SetParent(item.parent);
				newItem.localScale = Vector3.one;
				items.Add(enumerator.Current.Key, newItem);
                UpdateTechItem(newItem.GetComponent<TechItemViewModel>(), enumerator.Current.Key, enumerator.Current.Value);
            }

			return items;
		}

		private IEnumerable<LinkInfo> GetLinks(IDictionary<ITechnology, RectTransform> nodes)
		{
			foreach (var node in nodes)
			{
				foreach (var item in node.Key.Requirements)
				{
					RectTransform target;
					if (nodes.TryGetValue(item, out target))
					{
						var locked = !_research.IsTechResearched(item) || !_research.IsTechAvailable(item);
						yield return new LinkInfo(node.Value, target, locked);
					}
				}
			}
		}

		private void UpdateLinks(IEnumerable<LinkInfo> nodes)
		{
			var enumerator = nodes.GetEnumerator();
			RectTransform item = null;

			foreach (Transform transform in Content.transform)
			{
				var viewModel = transform.GetComponent<UiBezier>();
				if (viewModel == null)
					continue;
				
				item = viewModel.GetComponent<RectTransform>();
				if (enumerator.MoveNext())
				{
					transform.SetAsFirstSibling();
					UpdateLink(viewModel, enumerator.Current);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
			
			while (enumerator.MoveNext())
			{
				var newItem = (RectTransform)Instantiate(item);
				newItem.SetParent(item.parent);
				newItem.SetAsFirstSibling();
				newItem.localScale = Vector3.one;
				UpdateLink(newItem.GetComponent<UiBezier>(), enumerator.Current);
			}
		}

		private void UpdateTechItem(TechItemViewModel item, ITechnology model, Vector2 position)
		{
			item.gameObject.SetActive(true);
			item.Initialize(model);
			var rectTransform = item.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = position;
		}

		private void UpdateLink(UiBezier line, LinkInfo link)
		{
			var v1 = link.First.anchoredPosition;
			var v2 = link.Second.anchoredPosition;

			var size1 = link.First.rect.height/2 - BorderSize;
			var size2 = link.Second.rect.height/2 - BorderSize;
			var length = Vector2.Distance(v1,v2);

			if (v1.y > v2.y)
			{
				v1.y -= size1;
				v2.y += size2;
			}
			else
			{
				v1.y += size1;
				v2.y -= size2;
			}

			if (length > 0)
			{
				v1.x += size1*(v2.x - v1.x)/length;
				v2.x += size2*(v1.x - v2.x)/length;
			}

			line.SetPoints(v1, v2);
			line.color = link.Locked ? LockedColor : NormalColor;
		}

		private float _zoom = 1.0f;
		private ITechnology _selectedItem;

		private struct LinkInfo
		{
			public LinkInfo(RectTransform first, RectTransform second, bool locked)
			{
				First = first;
				Second = second;
				Locked = locked;
			}

			public readonly RectTransform First;
			public readonly RectTransform Second;
			public readonly bool Locked;
		}

		private static class TechTreeBulder
		{
			public static Dictionary<ITechnology, Vector2> Build(Faction faction, float Width, float Height, ITechnologies technologies, Research research)
			{
				var items = GetTechByLevel(faction, technologies, research);

				SortTree(items);

				var techTree = new Dictionary<ITechnology, Vector2>();
				foreach (var item in items)
					techTree.Add(item.Key, item.Value.ToVector2(Width, Height, item.Key.GetHashCode()));

				return techTree;
			}

			private static Dictionary<ITechnology, Position> GetTechByLevel(Faction faction, ITechnologies techDb, Research research)
			{
				var items = new Dictionary<ITechnology, Position>();
				var technologies = new HashSet<ITechnology>(techDb.All.OfFaction(faction).Where(item => !item.Special));
				var temp = new List<ITechnology>();
				
				int index = 0;
				while (technologies.Count > 0 && index < 1000)
				{
					temp.Clear();
					
					foreach (var tech in technologies)
						if (tech.Requirements.All(items.ContainsKey))
							temp.Add(tech);
					
					int x = 0;
					foreach (var item in temp)
					{
						items.Add(item, new Position(x++, index));
						technologies.Remove(item);
					}
					
					index++;
				}
				
				if (index >= 1000)
				{
					OptimizedDebug.LogError("BuildTechTree: bad tech data");
					OptimizedDebug.Break();
				}
				
				return items;
			}
			
			private static void SortTree(IDictionary<ITechnology, Position> technologies)
			{
				var nodes = new Dictionary<ITechnology, TechTreeNode>();
				var levels = new List<TechTreeNode>();
				var max = 0;
				foreach (var item in technologies)
				{
					var level = item.Value.y;
					while (levels.Count <= level)
						levels.Add(new TechTreeNode(null, -1, levels.Count));
					var head = levels[level];
					var last = head.Last;
					var node = new TechTreeNode(item.Key, last.x + 1, level);
					last.Add(node);
					nodes.Add(item.Key, node);

					max = Math.Max(max, node.x);
				}

				foreach (var level in levels)
				{
					var last = level.Last;
					while (last.x < max)
						last = last.Add(new TechTreeNode(null, last.x + 1, last.y));
				}

				foreach (var node in nodes)
				{
					foreach (var item in node.Key.Requirements)
					{
						var child = nodes[item];
						node.Value.Dependencies.Add(child);
						child.Dependencies.Add(node.Value);
					}
				}

				for (int i = 0; i < 10; ++i)
				{
					foreach (var level in levels)
					{
						var head = level;
						var current = head.Next;
						if (current == null)
							continue;

						while (current.Next != null)
						{
							var next = current.Next;
							var force1 = current.Force;
							var force2 = next.Force;

							//UnityEngine.OptimizedDebug.Log((current.Technology == null ? "null" : current.Technology.Id) + ":" + force1 + " > " + (next.Technology == null ? "null" : next.Technology.Id) + ":" + force2);

							if (force1 > 0 && (i < 5 || force1 > force2))
								current.MoveForward();
							else
								current = next;
						}

						current = current.Prev;
						while (current != head)
						{
							var next = current.Next;
							var prev = current.Prev;
							var force1 = current.Force;
							var force2 = next.Force;

							//UnityEngine.OptimizedDebug.Log((current.Technology == null ? "null" : current.Technology.Id) + ":" + force1 + " > " + (next.Technology == null ? "null" : next.Technology.Id) + ":" + force2);

							if (0 > force2 && (i < 5 || force1 > force2))
								current.MoveForward();

							current = prev;
						}
					}
				}

				foreach (var level in levels)
				{
					var item = level;
					while (item.Next != null)
					{
						item = item.Next;
						if (item.Technology == null)
							continue;

						technologies[item.Technology] = new Position(item.x, item.y);
					}
				}
			}

			private class TechTreeNode
			{
				public TechTreeNode(ITechnology technology, int x, int y)
				{
					Technology = technology;
					this.x = x;
					this.y = y;
				}

				public readonly ITechnology Technology;
				public int x;
				public int y;

				public void MoveForward()
				{
					Generic.Swap(ref x, ref Next.x);
					var prev = Prev;
					var next = Next;
					var nextnext = Next.Next;

					prev.Next = next;

					Prev = next;
					Next = nextnext;

					next.Prev = prev;
					next.Next = this;

					if (nextnext != null)
						nextnext.Prev = this;
				}

				public TechTreeNode Add(TechTreeNode node)
				{
					if (Next != null)
					{
						node.Next = Next;
						Next.Prev = node;
					}

					node.Prev = this;
					Next = node;

					return node;
				}

				public TechTreeNode Last
				{
					get
					{
						var temp = this;
						while (temp.Next != null)
							temp = temp.Next;
						return temp;
					}
				}

				public float Force
				{
					get
					{
						var force0 = CalculateForce(0);
						if (force0 > 0)
						{
							var force1 = CalculateForce(1);
							force0 = force0 - Math.Abs(force1);
							return Math.Max(0f, force0);
						}
						else
						{
							var force1 = CalculateForce(-1);
							force0 = force0 + Math.Abs(force1);
							return Math.Min(0f, force0);
						}
					}
				}

				private float CalculateForce(int dx)
				{
					float force = 0;
					foreach (var item in Dependencies)
					{
						var x1 = 2*item.x + item.y%2;
						var x2 = 2*(x+dx) + y%2;
						
						force += (float)(x1 - x2)/(1f + 0.4f*Math.Abs(y - item.y));
					}
					
					return force;
				}

				public TechTreeNode Next { get; private set; }
				public TechTreeNode Prev { get; private set; }

                public readonly List<TechTreeNode> Dependencies = new List<TechTreeNode>();
			}

			private struct Position
			{
				public Position(int x, int y) { this.x = x; this.y = y; }
				public readonly int x;
				public readonly int y;
				
				public Vector2 ToVector2(float Width, float Height, int seed)
				{
					UnityEngine.Random.seed = seed;
					var offset = UnityEngine.Random.insideUnitCircle;
					offset.x *= Width/5f;
					offset.y *= Height/5f;

					return new Vector2(x*Width + (1+y%2)*Width/2, y*Height + Height/2) + offset;
				}
			}
		}

        private Faction _faction = Faction.Undefined;
    }
}
