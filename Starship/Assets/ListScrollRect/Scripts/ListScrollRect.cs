using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Utils;

[ExecuteInEditMode]
public class ListScrollRect : UnityEngine.UI.ScrollRect
{
	#region Inner Classes

	// Class which holds information about each visible list item
	private class ListItemInfo
	{
		public int			index;
		public int			itemType;
		public GameObject	obj;
	}

	// Class which holds information about the list
	private class ListInfo
	{
		public float	currentTotalSize;
		public int		currentCount;
		public float	savedTotalSize;
		public int		savedCount;
		public bool		usedSavedSize;

		public int			scrollToIndex;
		public double		scrollingStart;
		public float		scrollFrom;
		public float		scrollTo;
		public float		scrollDelta;
		public float		scrollOffset;
		public ListAnchor	scrollAnchor;

		public float	CurrentAvgSize	{ get { return currentTotalSize / currentCount; } }
		public float	SavedAvgSize	{ get { return savedTotalSize / savedCount; } }

		public ListInfo()
		{
			Reset();
			ResetScroll();
		}

		public void Reset()
		{
			currentTotalSize	= 0;
			currentCount		= 0;
			savedTotalSize		= 0;
			savedCount			= 0;
			usedSavedSize		= false;
		}

		public void ResetScroll()
		{
			scrollToIndex	= -1;
			scrollingStart	= 0;
			scrollFrom		= 0;
			scrollTo		= 0;
			scrollDelta		= 0;
			scrollOffset	= 0;
		}
	}

	#endregion

	#region Enums

	public enum ScrollDirection
	{
		Vertical,
		Horizontal
	}

	public enum ListAnchor
	{
		Beginning,
		Middle,
		End
	}

	#endregion

	#region Inspector Variables

	[Tooltip("If true, ListScrollRect will call Initialize in the Start method.")]
	[SerializeField] private bool				initializeOnStart	= true;

	[Tooltip("The GameObject that has a component that implements the IContentFiller interface.")]
	[SerializeField] private GameObject			contentFillerInterface;
	
	[Tooltip("The direction the list will scroll.")]
	[SerializeField] private ScrollDirection 	scrollDir;
	
	[Tooltip("The padding that will be used in the list.")]
	[SerializeField] private float[]			padding	= new float[4];

	[Tooltip("The spacing between each item in the list.")]
	[SerializeField] private float				spacing;

	// Used by the editor only, disable the not used warning since we do use it in editor code.
	#pragma warning disable 0414
	[SerializeField, HideInInspector] private bool editorShowPadding = false;
	#pragma warning restore 0414

	#endregion

	#region Member Variables

	// Flag for initialization
	private bool					initialized;

	// The IContentFiller interface that will be used to populate the list
	private IContentFiller			contentFiller;

	// The number of items in this list, this is fetched from IContentFiller GetItemCount
	private int						itemCount;

	// Quick way to determine if the ListScrollRect is a vertical or horizontal list
	private bool					isVertical;

	// The GameObject which all pooled objects are placed as children, this GameObject is created at run time and is always in-active
	private GameObject				pooledObjContainer;

	// The list of visible items in the list scroll rect
	private List<ListItemInfo>		visibleObjs;

	// The list of pooled objects
	private List<ListItemInfo>		pooledObjs;

	// A dictionary used to keep track of the size of each items as we instantiate them, its used so when item sizes change for whatever
	// we can update the size of content and the positions of other visible items
	private Dictionary<int, float>	cachedSizes;

	// Used so when we are shifting items around as the user is dragging the list the content doesn't go all wonky
	private PointerEventData 		lastDragEventData;
	private int						endDragTempCount;

	// Used to calculate an estimate of the total size of the list
	private ListInfo				listInfo;

	// Used to determine when the content's width (if list is vertical) or height (if list is horizontal) has changed and we need to refresh the list
	private ListScrollRectHelper	listScrollRectHelper;
	private bool					markForRefresh;

	// Used for snapping
	private bool					checkSnaps;

	// The DrivenRectTransformTracker that highlights what RectTransform properties are being controlled by the ListScrollRect
	private DrivenRectTransformTracker tracker;

	#endregion

	#region Properties

	public bool				Initialized				{ get { return initialized; } }
	public GameObject		ContentFillerGameObject { get { return contentFillerInterface; } }
	public IContentFiller	ContentFillerInterface	{ get { return contentFiller; } }
	public ScrollDirection 	ScrollDir				{ get { return scrollDir; } }
	public float			PaddingLeft				{ get { return padding[0]; } }
	public float			PaddingRight			{ get { return padding[1]; } }
	public float			PaddingTop				{ get { return padding[2]; } }
	public float			PaddingBottom			{ get { return padding[3]; } }
	public float			Spacing					{ get { return spacing; } }
	public bool				MarkForRefresh			{ get { return markForRefresh; } set { markForRefresh = value; } }

	private static double SystemTimeInMilliseconds	{ get { return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalMilliseconds; } }

	// Returns the amount of contents space that appears above the viewport
	private float SpaceAbove { get { return -ContentPosition; } }

	// Returns the amount of contents space that appears below the viewport
	private float SpaceBelow { get { return ContentSize - SpaceAbove - ViewportSize; } }

	// Returns the position (x or y) of the content relative to the viewport
	private float ContentPosition
	{
		get { return isVertical ? -content.anchoredPosition.y : content.anchoredPosition.x; }
		set { content.anchoredPosition = isVertical ? new Vector2(content.anchoredPosition.x, -value) : new Vector2(value, content.anchoredPosition.y); }
	}

	// Returns the size (width or height) of the content
	private float ContentSize
	{
		get { return isVertical ? content.rect.height : content.rect.width; }
		set { content.sizeDelta = isVertical ? new Vector2(content.sizeDelta.x, value) : new Vector2(value, content.sizeDelta.y); }
	}

	// The position of the viewport relative to the top of content
	private float ViewportPosition { get { return -ContentPosition; } }

	// Returns the size (width or height) of the viewport
	private float ViewportSize
	{
		get
		{ 
			RectTransform rectT = (viewport == null) ? (RectTransform)transform : viewport;
			return isVertical ? rectT.rect.height : rectT.rect.width;
		}
	}

	#endregion

	#region Unity Methods

	protected override void Start()
	{
		base.Start();

		if (Application.isPlaying && initializeOnStart)
		{
			Initialize();
		}
	}

	protected virtual void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		if (markForRefresh)
		{
			RefreshContent();
			markForRefresh = false;
		}

		// Calculate any smooth scrolling that needs to happen
		if (listInfo.scrollToIndex != -1)
		{
			float itemSize = 0;

			listInfo.scrollTo = GetItemPosition(listInfo.scrollToIndex, out itemSize) + listInfo.scrollOffset;

			switch (listInfo.scrollAnchor)
			{
			case ListAnchor.Middle:
				listInfo.scrollTo -= (isVertical ? ViewportSize / 2f - itemSize / 2f : -ViewportSize / 2f + itemSize / 2f);
				break;
			case ListAnchor.End:
				listInfo.scrollTo -= (isVertical ? ViewportSize - itemSize : -ViewportSize + itemSize);
				break;
			}

			// Calculate the lerp using an ease out function
			double	timePassed		= SystemTimeInMilliseconds - listInfo.scrollingStart;
			float	delta			= listInfo.scrollDelta > 0 ? (float)(timePassed / listInfo.scrollDelta) : 1.0f;
			float	distanceLeft	= Mathf.Abs(listInfo.scrollTo - ViewportPosition);

			if (distanceLeft <= 0.5)
			{
				listInfo.ResetScroll();
			}
			else
			{
				ContentPosition = ContentPosition - Mathf.Lerp(0, distanceLeft, delta) * (listInfo.scrollTo < ViewportPosition ? -1 : 1);

				UpdateListItems();

				// Check if we have scrolled past the end of the content
				if (SpaceBelow < 0 || SpaceAbove < 0)
				{
					// Update the content position so its at the end
					ContentPosition = ContentPosition - (SpaceBelow < 0 ? SpaceBelow : -SpaceAbove);

					// Stop scrolling
					listInfo.ResetScroll();

					// Do one last update
					UpdateListItems();
				}
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		tracker.Clear();
	}

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		tracker.Clear();

		if (content != null && !Application.isPlaying)
		{
			DrivenTransformProperties drivenProperties = 
				((scrollDir == ScrollDirection.Vertical) ? DrivenTransformProperties.AnchoredPositionY : DrivenTransformProperties.AnchoredPositionX) |
				((scrollDir == ScrollDirection.Vertical) ? DrivenTransformProperties.AnchorMinY : DrivenTransformProperties.AnchorMinX) |
				((scrollDir == ScrollDirection.Vertical) ? DrivenTransformProperties.AnchorMaxY : DrivenTransformProperties.AnchorMaxX) |
				((scrollDir == ScrollDirection.Vertical) ? DrivenTransformProperties.PivotY : DrivenTransformProperties.PivotX) |
				((scrollDir == ScrollDirection.Vertical) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);

			tracker.Add(this, content, drivenProperties);

			SetupRectTransform(content);

			SetAnchoredPosition(content, 0);
			content.sizeDelta = new Vector2(scrollDir == ScrollDirection.Vertical ? content.sizeDelta.x : 0,
				scrollDir == ScrollDirection.Vertical ? 0 : content.sizeDelta.y);
		}
	}
	#endif

	#endregion

	#region Public Methods

	public bool Initialize()
	{
		if (initialized)
		{
			return true;
		}

	    if (contentFiller == null)
	    {
		    if (contentFillerInterface == null)
		    {
			    OptimizedDebug.LogError("ListScrollRect does not have an Content Filler Interface object attached to it.");
			    return false;
		    }

		    // Get the IContentFiller interface
		    contentFiller = contentFillerInterface.GetComponent(typeof(IContentFiller)) as IContentFiller;

		    if (contentFiller == null)
		    {
			    OptimizedDebug.LogError("ListScrollRect could not find a component on the Content Filler Interface object that implements the IContentFiller.");
			    return false;
		    }
	    }

        initialized = true;
		listInfo	= new ListInfo();
		visibleObjs	= new List<ListItemInfo>();
		pooledObjs	= new List<ListItemInfo>();
		cachedSizes	= new Dictionary<int, float>();

		onValueChanged.AddListener(Scrolled);

		// Create a GameObject to house all the pooled objects, this will always be in-active
		pooledObjContainer = new GameObject("PooledObjs");
		pooledObjContainer.SetActive(false);
		pooledObjContainer.transform.SetParent(transform);

		// Set any layout components on the content property that could interfere with ListScrollRect
		SetLayoutComponentsDisabled();

		// Populate the list with the info provided from IContentFiller
		RebuildContent();

		// Attach a ListScrollRectHelper component to the content gameObject to get the updates to the content
		if (content != null)
		{
			listScrollRectHelper = content.gameObject.AddComponent<ListScrollRectHelper>();
			listScrollRectHelper.Setup(this);
		}

		return true;
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent(IContentFiller contentFiller)
	{
		this.contentFiller = contentFiller;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			RebuildContent();
		}
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent(ScrollDirection scrollDir)
	{
		bool enableOther = (isVertical && horizontal) || (!isVertical && vertical);

		this.scrollDir = scrollDir;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			listScrollRectHelper.SetScrollDirection(scrollDir);
			RebuildContent();
		}

		if (isVertical)
		{
			horizontal = enableOther;
		}
		else
		{
			vertical = enableOther;
		}
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent(float paddingLeft, float paddingRight, float paddingTop, float paddingBottom, float spacing)
	{
		this.padding[0]		= paddingLeft;
		this.padding[1]		= paddingRight;
		this.padding[2]		= paddingTop;
		this.padding[3]		= paddingBottom;
		this.spacing		= spacing;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			RebuildContent();
		}
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent(ScrollDirection scrollDir, float paddingLeft, float paddingRight, float paddingTop, float paddingBottom, float spacing)
	{
		this.scrollDir		= scrollDir;
		this.padding[0]		= paddingLeft;
		this.padding[1]		= paddingRight;
		this.padding[2]		= paddingTop;
		this.padding[3]		= paddingBottom;
		this.spacing		= spacing;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			listScrollRectHelper.SetScrollDirection(scrollDir);
			RebuildContent();
		}
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent(IContentFiller contentFiller, ScrollDirection scrollDir, float paddingLeft, float paddingRight, float paddingTop, float paddingBottom, float spacing)
	{
		this.contentFiller	= contentFiller;
		this.scrollDir		= scrollDir;
		this.padding[0]		= paddingLeft;
		this.padding[1]		= paddingRight;
		this.padding[2]		= paddingTop;
		this.padding[3]		= paddingBottom;
		this.spacing		= spacing;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			RebuildContent();
			listScrollRectHelper.SetScrollDirection(scrollDir);
		}
	}

	/// <summary>
	/// Destroys all pooled list item GameObjects and calls IContentFiller GetListItem to re-instantiate them all. Should be called if the objects(s) used for the list items changes. 
	/// </summary>
	public void RebuildContent()
	{
		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else if (contentFiller == null)
		{
			OptimizedDebug.LogError("ListScrollRect does not have an IContentFiller script attached to it.");
		}
		else
		{
			DestroyAllPoolObjects();

			isVertical = (scrollDir == ScrollDirection.Vertical);

			if (isVertical)
			{
				vertical = true;
			}
			else
			{
				horizontal = true;
			}

			// Setup the RectTransform for the content
			SetupRectTransform(content);

			RefreshContent(0, 0);

			listInfo.ResetScroll();
		}
	}

	/// <summary>
	/// Returns all list item GameObjects back to the pool and called IContentFiller GetListItem for each visible item. Should be called if the content in a visible list item changes.
	/// </summary>
	public void RefreshContent(float paddingLeft, float paddingRight, float paddingTop, float paddingBottom, float spacing)
	{
		this.padding[0]		= paddingLeft;
		this.padding[1]		= paddingRight;
		this.padding[2]		= paddingTop;
		this.padding[3]		= paddingBottom;
		this.spacing		= spacing;

		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			RefreshContent();
		}
	}

	/// <summary>
	/// Returns all list item GameObjects back to the pool and called IContentFiller GetListItem for each visible item. Should be called if the content in a visible list item changes.
	/// </summary>
	public void RefreshContent()
	{
		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			float	topPosValue		= isVertical ? content.anchoredPosition.y : -content.anchoredPosition.x;
			int		lowestIndex		= (visibleObjs.Count == 0) ? 0 : int.MaxValue;
			float	contentOffset	= 0; 

			for (int i = 0; i < visibleObjs.Count; i++)
			{
				if (lowestIndex > visibleObjs[i].index)
				{
					RectTransform objRectT	= visibleObjs[i].obj.transform as RectTransform;
					lowestIndex				= visibleObjs[i].index;
					contentOffset			= topPosValue - (isVertical ? -objRectT.anchoredPosition.y : objRectT.anchoredPosition.x);
				}
			}

			// At this point contentOffset will include the padding but we do not it to, so we negate it
			contentOffset += (isVertical ? PaddingTop : PaddingLeft);
			contentOffset *= (isVertical ? 1 : -1);

			RefreshContent(lowestIndex, contentOffset + (isVertical ? 1 : -1) * (lowestIndex != 0 ? spacing : 0));

			listInfo.ResetScroll();
		}
	}

	/// <summary>
	/// Puts the list item at "itemIndex" at the front of the list (top of list if vertical, left of list if horizontal)
	/// </summary>
	public void GoToListItem(int itemIndex)
	{
		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}

	    if (itemCount == 0)
	    {
	        return;
	    }

		if (itemIndex < 0)
		{
			throw new System.ArgumentOutOfRangeException("itemIndex", string.Format("[ListScrollRect] GoToListItem: itemIndex ({0}) must be greater than of equal to 0.", itemIndex));
		}
		else if(itemIndex >= itemCount)
		{
			throw new System.ArgumentOutOfRangeException("itemIndex", string.Format("[ListScrollRect] GoToListItem: itemIndex ({0}) must be less than the item count ({1}).", itemIndex, itemCount));
		}

		// Need to shift the content by spacing so the item at itemIndex appears at the top/left of the list
		float contentOffset = (isVertical ? PaddingTop : -PaddingLeft) + (isVertical ? 1 : -1) * (itemIndex != 0 ? spacing : 0);

		RefreshContent(itemIndex, contentOffset);
	}

	/// <summary>
	/// Scrolls the list to the list item at the given index.
	/// </summary>
	/// <param name="itemIndex">Item index.</param>
	/// <param name="scrollDelta">Controls the speed at which the list scrolls.</param>
	public void ScrollToListItem(int itemIndex, ListAnchor anchor = ListAnchor.Beginning, float offset = 0, float scrollDelta = 10000)
	{
		// Make sure we are initialized
		if (!initialized)
		{
			Initialize();
		}

		if (itemIndex < 0)
		{
			throw new System.ArgumentOutOfRangeException("itemIndex", string.Format("[ListScrollRect] ScrollToListItem: itemIndex ({0}) must be greater than of equal to 0.", itemIndex));
		}
		else if(itemIndex >= itemCount)
		{
			throw new System.ArgumentOutOfRangeException("itemIndex", string.Format("[ListScrollRect] ScrollToListItem: itemIndex ({0}) must be less than the item count ({1}).", itemIndex, itemCount));
		}

		listInfo.ResetScroll();

		listInfo.scrollAnchor	= anchor;
		listInfo.scrollOffset	= offset;
		listInfo.scrollDelta	= scrollDelta;
		listInfo.scrollToIndex	= itemIndex;
		listInfo.scrollingStart	= SystemTimeInMilliseconds;
		listInfo.scrollFrom		= ViewportPosition;

        Update();
	}

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		lastDragEventData		= eventData;

		// Stop the scrolling
		listInfo.ResetScroll();
	}

	/// <summary>
	/// Raises the end drag event.
	/// </summary>
	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);

		lastDragEventData = null;
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Called when content in the ScrollRect is scrolled
	/// </summary>
	private void Scrolled(Vector2 scrollPos)
	{
		if (!Application.isPlaying || content == null || contentFiller == null || listInfo.scrollToIndex != -1)
		{
			return;
		}

		// If there are no visible objects but the itemCount is greater than 0 then something very bad happened. If there are no visible items in the list
		// we have no way to know what items should be appearing in the list at this time so we are going to rebuild the list from the beginning. This is
		// a fail safe.
		if (visibleObjs.Count == 0 && itemCount > 0)
		{
			OptimizedDebug.LogWarning("Something bad happened and now there are no visible items in the list. Since we have no idea what items need to be generate we are going to rebuild the list from the beginning.");
			RebuildContent();
			return;
		}

		UpdateListItems();
	}

	/// <summary>
	/// Returns all items to the pool, re-initializes the ListScrollRect with any new values, and re-generates all visible list items.
	/// </summary>
	private void RefreshContent(int itemIndex, float contentOffset)
	{
		if (content == null || contentFiller == null)
		{
			return;
		}

		EndDrag();

		// Return all visible items to the pool
		ReturnAllToPool();

		// Clear all cached item sizes so we can start fresh
		cachedSizes.Clear();

		// Set the size of content back to its min size (ie. size with no items in it)
		content.sizeDelta = new Vector2(isVertical ? content.sizeDelta.x : PaddingLeft + PaddingRight, isVertical ? PaddingTop + PaddingBottom : content.sizeDelta.y);

		// Set the position of the content back to 0 (so its at the top)
		SetAnchoredPosition(content, 0);

		// Set the values back to default
		listInfo.Reset();

		// Get the item count from the IContentFiller
		itemCount = contentFiller.GetItemCount();

		// If there are no items then bail out here
		if (itemCount == 0)
		{
			return;
		}

		// Check the itemIndex is still in bounds
		if (itemIndex >= itemCount)
		{
			itemIndex		= itemCount - 1;
			contentOffset	= 0;
		}

		// Shift the content by contentOffset, this is used so we can keep the content in the same spot after the refresh
		if (contentOffset != 0)
		{
			ShiftAnchoredPosition(content, contentOffset);
		}

		// Start generating all the items starting at index itemIndex
		GenerateListItem(itemIndex, isVertical ? -PaddingTop : PaddingLeft, false);

		// Add some height for a space if we generated items starting at an index != 0
		if (itemIndex != 0)
		{
			float width		= isVertical ? content.sizeDelta.x : content.sizeDelta.x + spacing;
			float height	= isVertical ? content.sizeDelta.y + spacing : content.sizeDelta.y;

			content.sizeDelta = new Vector2(width, height);
		}

		float val1;
		float diff;

		// Get the amount of "extra" space in the scroll area that is not filled by a list item.
		if (isVertical)
		{
			val1 = content.rect.height - content.anchoredPosition.y;
			diff = ViewportSize - val1;
		}
		else
		{
			val1 = content.rect.width + content.anchoredPosition.x;
			diff = ViewportSize - val1;
		}

		// If there is "extra" space then try and fill it with items by shifting the content and checking for new items.
		if (diff > 0)
		{
			// Shift it so the content is at the end
			ShiftAnchoredPosition(content, isVertical ? -diff : diff);

			// Generate any new list items
			UpdateListItems();

			// If the contents position is less than 0 then we need to shift it back up
			if ((isVertical && content.anchoredPosition.y < 0) ||
			    (!isVertical && content.anchoredPosition.x > 0))
			{
				ShiftAnchoredPosition(content, -content.anchoredPosition.y);
			}
		}

		// Save the average size of the current visible items
		listInfo.savedTotalSize = listInfo.currentTotalSize;
		listInfo.savedCount		= listInfo.currentCount;
		listInfo.usedSavedSize	= true;

		// Now we are going to calcualte the estimated size of the content using the adverage of the list items sizes we have seen so far
		int lowestIndex		= int.MaxValue;
		int highestIndex	= int.MinValue;

		for (int i = 0; i < visibleObjs.Count; i++)
		{
			lowestIndex		= System.Math.Min(lowestIndex, visibleObjs[i].index);
			highestIndex	= System.Math.Max(highestIndex, visibleObjs[i].index);
		}

		// This handles the edge case where items were generated before lowestIndex but were returned to the pool cause they were generated off screen
		while (cachedSizes.ContainsKey(lowestIndex - 1))
		{
			lowestIndex--;
		}

		float extraSize1 = 0;
		float extraSize2 = 0;

		// Calcuate the space that goes at the beginning
		if (lowestIndex > 0)
		{
			extraSize1 += listInfo.SavedAvgSize * lowestIndex;
		}

		// Calcuate the space that goes at the end
		if (highestIndex < itemCount - 1)
		{
			extraSize2 += listInfo.SavedAvgSize * (itemCount - highestIndex - 1);
		}

		if (extraSize1 + extraSize2 > 0)
		{
			// Resize the content
			content.sizeDelta = (isVertical ? new Vector2(content.sizeDelta.x, content.sizeDelta.y + extraSize1 + extraSize2) : new Vector2(content.sizeDelta.x + extraSize1 + extraSize2, content.sizeDelta.y));

			// If we add space at the beginning we need to shift the content so the items on the screen do not move
			if (extraSize1 > 0)
			{
				ShiftContentBy(isVertical ? extraSize1 : -extraSize1);
			}
		}


		BeginDrag();
	}

	/// <summary>
	/// Creates a new list item to be positioned in the list.
	/// </summary>
	private void GenerateListItem(int index, float position, bool topItem)
	{
		int				itemType		= contentFiller.GetItemType(index);
		ListItemInfo	listItemInfo	= GetPooledObj(itemType);

		// Get the list items object and set up some parameters
		listItemInfo.index	= index;
		listItemInfo.obj	= contentFiller.GetListItem(index, itemType, listItemInfo.obj);

		if (listItemInfo.obj == null)
		{
			OptimizedDebug.LogErrorFormat("GetListItem returned a null object for item at index {0}", index);
			return;
		}

		listItemInfo.obj.SetActive(true);
		listItemInfo.obj.transform.SetParent(content, false);

		SetupRectTransform(listItemInfo.obj.transform as RectTransform);

		visibleObjs.Add(listItemInfo);

		if (!CanvasUpdateRegistry.IsRebuildingLayout())
		{
			Canvas.ForceUpdateCanvases();
		}

		PositionListItem(listItemInfo, position, topItem);

		UpdateListItems();
	}

	/// <summary>
	/// Returns any objects no longer visible to the pool and generates new items at the front/back of the list.
	/// </summary>
	private bool UpdateListItems()
	{
        if (++_recursionCounter > 50)
        {
			OptimizedDebug.LogException(new InvalidOperationException("UpdateListItems - infinite recursion"));
            _recursionCounter = 0;
            return false;
        }

		float	topPosValue		= isVertical ? content.anchoredPosition.y : -content.anchoredPosition.x;
		float	bottomPosValue	= topPosValue + ViewportSize;

		int		highestIndex	= int.MaxValue;
		int 	lowestIndex		= int.MinValue;

		float	highestVal		= float.MinValue;
		float	lowestVal		= float.MaxValue;

		for (int i = visibleObjs.Count - 1; i >= 0; i--)
		{
			ListItemInfo	listItemInfo	= visibleObjs[i];
			RectTransform	childRectT		= listItemInfo.obj.transform as RectTransform;
			
			float val1 = isVertical ? -1 * childRectT.anchoredPosition.y : childRectT.anchoredPosition.x;
			float val2 = isVertical ? childRectT.rect.height : childRectT.rect.width;
			
			if (val1 < lowestVal)
			{
				lowestVal		= val1;
				lowestIndex		= listItemInfo.index;
			}
			else if (val1 == lowestVal && lowestIndex > listItemInfo.index)
			{
				lowestIndex = listItemInfo.index;
			}
			
			if (val1 + val2 > highestVal)
			{
				highestVal		= val1 + val2;
				highestIndex	= listItemInfo.index;
			}
			else if (val1 + val2 == highestVal && highestIndex < listItemInfo.index)
			{
				highestIndex = listItemInfo.index;
			}

			if (val1 + val2 <= topPosValue || val1 >= bottomPosValue)
			{
				ReturnPooledObj(listItemInfo);
			}
		}

		// Check if content has scrolled so much that the front most item has scolled past the end of the viewport
		if (lowestVal >= bottomPosValue)
		{
			// Calculate how many items we think will go in that gap
			float	gapSize			= lowestVal - topPosValue;
			int		numItemsInGap	= Mathf.CeilToInt(gapSize / listInfo.CurrentAvgSize);

			// Calculate what item to go to
			int gotoIndex = lowestIndex - numItemsInGap;

			// Check if that item is greater than the max items
			if (gotoIndex < 0)
			{
				gotoIndex = 0;
			}

			// Jump to that item. We now simulate the "scroll"
			GoToListItem(gotoIndex);

            _recursionCounter--;
			return true;
		}

		// Check if an item should be generated at the front of the list
		if (lowestVal - spacing > topPosValue && lowestIndex > 0)
		{
			GenerateListItem(lowestIndex - 1, isVertical ? -lowestVal : lowestVal, true);

            _recursionCounter--;
			return true;
		}

		// Check if we have scrolled so much that the end most item has scolled past the beginning of the viewport
		if (highestVal <= topPosValue)
		{
			// Calculate how many items we think will go in that gap
			float	gapSize			= topPosValue - highestVal;
			int		numItemsInGap	= Mathf.CeilToInt(gapSize / listInfo.CurrentAvgSize);

			// Calculate what item to go to
			int gotoIndex = highestIndex + numItemsInGap;

			// Check if that item is greater than the max items
			if (gotoIndex >= itemCount)
			{
				gotoIndex = itemCount - 1;
			}

			// Jump to that item. We now simulate the "scroll"
			GoToListItem(gotoIndex);

            _recursionCounter--;
			return true;
		}

		// Check if an item should be generated at the end of the list
		if (highestVal + spacing < bottomPosValue && highestIndex < itemCount - 1)
		{
			GenerateListItem(highestIndex + 1, isVertical ? -highestVal : highestVal, false);

            _recursionCounter--;
			return true;
		}

        _recursionCounter--;
		return false;
	}

    private int _recursionCounter = 0;

	// Positions the scroll item in the proper place in the content
	private void PositionListItem(ListItemInfo listItemInfo, float listPos, bool addedItemAtBeginning)
	{
		// Get the items size
		RectTransform	objRectT	= listItemInfo.obj.transform as RectTransform;
		float			objSize		= isVertical ? objRectT.rect.height : objRectT.rect.width;

		// Need to account for the spacing between items
		float spacingOffset	= 0;

		if (addedItemAtBeginning)
		{
			spacingOffset = isVertical ? spacing : -spacing;
		}
		else if (listItemInfo.index != 0)
		{
			spacingOffset = isVertical ? -spacing : spacing;
		}

		float sizeOffset	= (addedItemAtBeginning ? objSize : 0);
		float pos			= isVertical ? listPos + spacingOffset + sizeOffset : listPos + spacingOffset - sizeOffset;
		
		SetAnchoredPosition(objRectT, pos);

		// If the objects size doesn't match its cached size then we need to re-size the contents RectTransform
		if (!cachedSizes.ContainsKey(listItemInfo.index) || objSize != cachedSizes[listItemInfo.index])
		{
			float	defaultSize		= listInfo.usedSavedSize ? listInfo.SavedAvgSize : 0;
			float	sizeDifference	= objSize - (!cachedSizes.ContainsKey(listItemInfo.index) ? defaultSize : cachedSizes[listItemInfo.index]);
			Vector2	contentsNewSize	= new Vector2(content.sizeDelta.x, content.sizeDelta.y);
			bool	includeSpacing	= (!cachedSizes.ContainsKey(listItemInfo.index) && listItemInfo.index != itemCount - 1);
			
			if (isVertical)
			{
				contentsNewSize.y += sizeDifference + (includeSpacing ? this.spacing : 0);
			}
			else
			{
				contentsNewSize.x += sizeDifference + (includeSpacing ? this.spacing : 0);
			}

			content.sizeDelta = contentsNewSize;

			if (addedItemAtBeginning)
			{
				sizeDifference += (!cachedSizes.ContainsKey(listItemInfo.index) && listItemInfo.index != 0) ? spacing : 0;
				sizeDifference *= isVertical ? 1 : -1;
				ShiftContentBy(sizeDifference);
			}

			// Negate the old size then add the new size (we are not using sizeDifference for a reason, ei. defaultSize)
			listInfo.currentTotalSize	-= !cachedSizes.ContainsKey(listItemInfo.index) ? 0 : cachedSizes[listItemInfo.index];
			listInfo.currentTotalSize	+= objSize;
			listInfo.currentCount		+= !cachedSizes.ContainsKey(listItemInfo.index) ? 1 : 0;

			cachedSizes[listItemInfo.index] = objSize;
		}
	}

	// Helper function that shifts the content RectTransform and all visible items so that the items appear to stay in the same place.
	private void ShiftContentBy(float amount)
	{
		EndDrag();

		// Shift the content
		ShiftAnchoredPosition(content, amount);

		// Shift all items in the content so it looks like they didnt move
		for (int i = 0; i < visibleObjs.Count; i++)
		{
			ShiftAnchoredPosition(visibleObjs[i].obj.transform as RectTransform, -amount);
		}

		BeginDrag();
	}

	// Get the estimated position of the list item in the content, if it found the item in the list of visible objects then size will be the widht/height of the item
	private float GetItemPosition(int itemIndex, out float size)
	{
		int lowestIndex		= int.MaxValue;
		int highestIndex	= int.MinValue;

		float lowestPos		= 0;
		float highestPos	= 0;

		for (int i = 0; i < visibleObjs.Count; i++)
		{
			RectTransform	objRectT	= visibleObjs[i].obj.transform as RectTransform;
			float			objPos		= (isVertical ? -objRectT.anchoredPosition.y : objRectT.anchoredPosition.x);

			if (itemIndex == visibleObjs[i].index)
			{
				size = isVertical ? objRectT.rect.height : objRectT.rect.width;
				return objPos;
			}

			if (lowestIndex > visibleObjs[i].index)
			{
				lowestIndex	= visibleObjs[i].index;
				lowestPos	= objPos;
			}

			if (highestIndex < visibleObjs[i].index)
			{
				highestIndex	= visibleObjs[i].index;
				highestPos		= objPos;
			}
		}

		size = 0;

		if (itemIndex < lowestIndex)
		{
			return lowestPos - listInfo.CurrentAvgSize * (lowestIndex - itemIndex);
		}
		else if (itemIndex > highestIndex)
		{
			return highestPos + listInfo.CurrentAvgSize * (itemIndex - highestIndex);
		}

		return 0;
	}

	// Gets a pooled object, if there are no available pooled objects then create a ListItemInfo with a null obj so one can
	// be created in IContentFilled GetListItem.
	private ListItemInfo GetPooledObj(int itemType)
	{
		ListItemInfo listItemInfo = null;
		
		for (int i = 0; i < pooledObjs.Count; i++)
		{
			if (itemType == pooledObjs[i].itemType)
			{
				listItemInfo = pooledObjs[i];
				pooledObjs.RemoveAt(i);
				break;
			}
		}
		
		if (listItemInfo == null)
		{
			listItemInfo			= new ListItemInfo();
			listItemInfo.itemType	= itemType;
		}
		
		return listItemInfo;
	}

	// Returns an item to the pool to be re-used later
	private void ReturnPooledObj(ListItemInfo listItemInfo)
	{
		listItemInfo.obj.SetActive(false);
		listItemInfo.obj.transform.SetParent(pooledObjContainer.transform, false);
		visibleObjs.Remove(listItemInfo);
		pooledObjs.Add(listItemInfo);
	}

	/// <summary>
	/// Destroys all visible and pooled objects
	/// </summary>
	private void DestroyAllPoolObjects()
	{
		for (int i = 0; i < visibleObjs.Count; i++)
		{
			GameObject.Destroy(visibleObjs[i].obj);
		}
		
		for (int i = 0; i < pooledObjs.Count; i++)
		{
			GameObject.Destroy(pooledObjs[i].obj);
		}
		
		visibleObjs.Clear();
		pooledObjs.Clear();
	}

	/// <summary>
	/// Returns all to items to the pool.
	/// </summary>
	private void ReturnAllToPool()
	{
		for (int i = visibleObjs.Count - 1; i >= 0; i--)
		{
			ReturnPooledObj(visibleObjs[i]);
		}
	}

	private void EndDrag()
	{
		if (lastDragEventData == null)
		{
			return;
		}

		if (endDragTempCount == 0)
		{
			base.OnEndDrag(lastDragEventData);
		}

		endDragTempCount++;
	}

	private void BeginDrag()
	{
		if (lastDragEventData == null || endDragTempCount == 0)
		{
			return;
		}

		endDragTempCount--;

		if (endDragTempCount == 0)
		{
			base.OnBeginDrag(lastDragEventData);
		}
	}

	/// <summary>
	/// Helper method used to move the anchoredPosition of a RectTransform by an amount.
	/// </summary>
	private void ShiftAnchoredPosition(RectTransform rectT, float amount)
	{
		Vector2 anchoredPosition = new Vector2();

		if (scrollDir == ScrollDirection.Vertical)
		{
			anchoredPosition.x = rectT.anchoredPosition.x;
			anchoredPosition.y = rectT.anchoredPosition.y + amount;
		}
		else
		{
			anchoredPosition.x = rectT.anchoredPosition.x + amount;
			anchoredPosition.y = rectT.anchoredPosition.y;
		}
		
		rectT.anchoredPosition = anchoredPosition;
	}

	/// <summary>
	/// Helper method used to set the anchoredPosition of a RectTransform
	/// </summary>
	private void SetAnchoredPosition(RectTransform rectT, float newPos)
	{
		Vector2 anchoredPosition = new Vector2();

		if (scrollDir == ScrollDirection.Vertical)
		{
			anchoredPosition.x = rectT.anchoredPosition.x;
			anchoredPosition.y = newPos;
		}
		else
		{
			anchoredPosition.x = newPos;
			anchoredPosition.y = rectT.anchoredPosition.y;
		}

		rectT.anchoredPosition = anchoredPosition;
	}

	/// <summary>
	/// Sets the anchors and pivot of a RectTransform to what it needs to be to properly place items in the list
	/// </summary>
	private void SetupRectTransform(RectTransform rectT)
	{
		if (scrollDir == ScrollDirection.Vertical)
		{
			rectT.anchorMin	= new Vector2(rectT.anchorMin.x, 1);
			rectT.anchorMax	= new Vector2(rectT.anchorMax.x, 1);
			rectT.pivot		= new Vector2(rectT.pivot.x, 1);
		}
		else
		{
			rectT.anchorMin	= new Vector2(0, rectT.anchorMin.y);
			rectT.anchorMax	= new Vector2(0, rectT.anchorMax.y);
			rectT.pivot		= new Vector2(0, rectT.pivot.y);
		}
	}

	/// <summary>
	/// Disables any LayoutGroups or ContentSizeFitter on the content since ListScrollRect will be controlling the size and position of content.
	/// </summary>
	private void SetLayoutComponentsDisabled()
	{
		LayoutGroup lg = content.GetComponent<LayoutGroup>();
		if (lg != null)
		{
			OptimizedDebug.LogWarning("ListScrollRect: Found a LayoutGroup component on the ScrollRect content. ListScrollRect controls the layout of the content. Disabling LayoutGroup.");
			lg.enabled = false;
		}
		
		ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
		if (csf != null)
		{
			OptimizedDebug.LogWarning("ListScrollRect: Found a ContentSizeFitter component on the ScrollRect content. ListScrollRect controls the size of the content. Disabling ContentSizeFitter.");
			csf.enabled = false;
		}
	}

	#endregion
}
