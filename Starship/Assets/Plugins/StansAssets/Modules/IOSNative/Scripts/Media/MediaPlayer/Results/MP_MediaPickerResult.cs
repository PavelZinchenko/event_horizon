////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MP_MediaPickerResult : SA.Common.Models.Result {

	private List<MP_MediaItem> _SelectedmediaItems;


	public MP_MediaPickerResult(List<MP_MediaItem> selectedItems):base() {
		_SelectedmediaItems = selectedItems;
	}

	public MP_MediaPickerResult(string errorData):base(new SA.Common.Models.Error(errorData)) {
		
	}



	public List<MP_MediaItem> SelectedmediaItems {
		get {
			return _SelectedmediaItems;
		}
	}

	public List<MP_MediaItem> Items {
		get {
			return SelectedmediaItems;
		}
	}
}
