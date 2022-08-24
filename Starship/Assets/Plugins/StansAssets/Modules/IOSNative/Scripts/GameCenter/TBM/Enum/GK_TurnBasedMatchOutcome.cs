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

public enum GK_TurnBasedMatchOutcome  {
	None         = 0,        // Participants who are not done with a match have this state
	Quit         = 1,        // Participant quit
	Won          = 2,        // Participant won
	Lost         = 3,        // Participant lost
	Tied         = 4,        // Participant tied
	TimeExpired  = 5,        // Game ended due to time running out
	First        = 6,
	Second       = 7,
	Third        = 8,
	Fourth       = 9
}
