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

public enum GK_TurnBasedParticipantStatus  {
	Unknown     = 0,
	Invited     = 1,    // a participant has been invited but not yet responded
	Declined    = 2,    // a participant that has declined an invite to this match
	Matching    = 3,    // a participant that is waiting to be matched
	Active      = 4,    // a participant that is active in this match
	Done        = 5,    // a participant is done with this session
}
