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

public class GK_SavesResolveResult : SA.Common.Models.Result {

	private List<GK_SavedGame> _ResolvedSaves = new List<GK_SavedGame>();

	public GK_SavesResolveResult(List<GK_SavedGame> saves):base() {
		_ResolvedSaves = saves;
	}
	
	public GK_SavesResolveResult(SA.Common.Models.Error error):base(error) {}

	public GK_SavesResolveResult(string errorData):base(new SA.Common.Models.Error(errorData)) {} 


	/// <summary>
	/// An array of GK_SavedGame objects containing a saved game list with all conflicts contained in the
	/// conflicting parameter resolved. If there are any conflicting saved game files that were not in the
	/// conflicts parameter, these objects will automatically be appended to the end of the SavedGames
	/// list. For example, if there are five saved game files with the same name, but only three are in the
	/// conflicts array, this array will contain the resolved saved game file and the two unresolved files.
	/// </summary>
	public List<GK_SavedGame> SavedGames {
		get {
			return _ResolvedSaves;
		}
	}
}
