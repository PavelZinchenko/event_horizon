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

public class IOSDialog : BaseIOSPopup {
	

	public string yes;
	public string no;

	public event Action<IOSDialogResult> OnComplete = delegate {};
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static IOSDialog Create(string title, string message) {
		return Create(title, message, "Yes", "No");
	}
		
	public static IOSDialog Create(string title, string message, string yes, string no) {
		IOSDialog dialog;
		dialog  = new GameObject("IOSPopUp").AddComponent<IOSDialog>();
		dialog.title = title;
		dialog.message = message;
		dialog.yes = yes;
		dialog.no = no;
		dialog.init();
		
		return dialog;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void init() {
		IOSNativePopUpManager.showDialog(title, message, yes, no);
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
				OnComplete(IOSDialogResult.YES);
				break;
			case 1: 
				OnComplete(IOSDialogResult.NO);
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
