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

public class IOSMessage : BaseIOSPopup {
	
	
	public string ok;
	public event Action OnComplete = delegate {};
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static IOSMessage Create(string title, string message) {
		return Create(title, message, "Ok");
	}
		
	public static IOSMessage Create(string title, string message, string ok) {
		IOSMessage dialog;
		dialog  = new GameObject("IOSPopUp").AddComponent<IOSMessage>();
		dialog.title = title;
		dialog.message = message;
		dialog.ok = ok;
		
		dialog.init();
		
		return dialog;
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void init() {
		IOSNativePopUpManager.showMessage(title, message, ok);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	public void onPopUpCallBack(string buttonIndex) {

		OnComplete();
		Destroy(gameObject);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
