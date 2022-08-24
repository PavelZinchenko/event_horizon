////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IOSRateUsPopUp : BaseIOSPopup {
	
	public string rate;
	public string remind;
	public string declined;


	public event Action<IOSDialogResult> OnComplete = delegate {};


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public static IOSRateUsPopUp Create() {
		return Create("Like the Game?", "Rate Us");
	}
	
	public static IOSRateUsPopUp Create(string title, string message) {
		return Create(title, message, "Rate Now", "Ask me later", "No, thanks");
	}
	
	public static IOSRateUsPopUp Create(string title, string message, string rate, string remind, string declined) {
		IOSRateUsPopUp popup = new GameObject("IOSRateUsPopUp").AddComponent<IOSRateUsPopUp>();
		popup.title = title;
		popup.message = message;
		popup.rate = rate;
		popup.remind = remind;
		popup.declined = declined;
		
		popup.init();
		
	
		return popup;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public void init() {
		IOSNativePopUpManager.showRateUsPopUp(title, message, rate, remind, declined);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	public void onPopUpCallBack(string buttonIndex) {
		int index = System.Convert.ToInt16(buttonIndex);
		switch(index) {
			case 0: 
				IOSNativeUtility.RedirectToAppStoreRatingPage();
				OnComplete(IOSDialogResult.RATED);
				break;
			case 1:
				OnComplete(IOSDialogResult.REMIND);
				break;
			case 2:
				OnComplete(IOSDialogResult.DECLINED);
				break;
		}
		
		
		
		Destroy(gameObject);
	} 
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
