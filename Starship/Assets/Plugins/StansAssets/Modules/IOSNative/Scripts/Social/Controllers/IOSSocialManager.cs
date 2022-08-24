//#define SOCIAL_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSSocialManager : SA.Common.Pattern.Singleton<IOSSocialManager> {

	#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_TwPost(string text, string url, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_TwPostGIF(string text, string url);

	[DllImport ("__Internal")]
	private static extern void _ISN_FbPost(string text, string url, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_MediaShare(string text, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_SendMail(string subject, string body,  string recipients, string encodedMedia);


	[DllImport ("__Internal")]
	private static extern void _ISN_InstaShare(string encodedMedia, string message);


	[DllImport ("__Internal")]
	private static extern void _ISN_WP_ShareText(string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_WP_ShareMedia(string encodedMedia);



	[DllImport ("__Internal")]
	private static extern void _ISN_SendTextMessage(string body, string recipients);


	#endif
	


	//Actions

	public static event Action OnFacebookPostStart = delegate {};
	public static event Action OnTwitterPostStart = delegate {};
	public static event Action OnInstagramPostStart = delegate {};


	public static event Action<SA.Common.Models.Result> OnFacebookPostResult = delegate {};
	public static event Action<SA.Common.Models.Result> OnTwitterPostResult = delegate {};
	public static event Action<SA.Common.Models.Result> OnInstagramPostResult = delegate {};
	public static event Action<SA.Common.Models.Result> OnMailResult = delegate {};



	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void ShareMedia(string text, Texture2D texture = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			if(texture != null) {
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
				_ISN_MediaShare(text, bytesString);
			} else {
				_ISN_MediaShare(text, "");
			}
		#endif
	}

	public void TwitterPost(string text, string url = null, Texture2D texture = null) {
		OnTwitterPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}

		if(url == null) {
			url = "";
		}

		string encodedMedia = "";

		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}


		_ISN_TwPost(text, url, encodedMedia);
		#endif
	}


	public void TwitterPostGif(string text, string url) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		_ISN_TwPostGIF(text, url);
		#endif
	}


	public void FacebookPost(string text, string url = null, Texture2D texture = null) {
		OnFacebookPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}
		
		if(url == null) {
			url = "";
		}
		
		string encodedMedia = "";
		
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}
		
		
		_ISN_FbPost(text, url, encodedMedia);
		#endif
	}


	public void InstagramPost(Texture2D texture) {
		InstagramPost(texture, "");
	}
	
	
	public void InstagramPost(Texture2D texture, string message) {
		OnInstagramPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		
		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);
		
		
		_ISN_InstaShare(bytesString, message);
		
		#endif
		
	}


	public void WhatsAppShareText(string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		_ISN_WP_ShareText(message);
		#endif
	}


	public void WhatsAppShareImage(Texture2D texture) {

		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE

		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		_ISN_WP_ShareMedia(bytesString);

		#endif
	}



	public void SendMail(string subject, string body, string recipients) {
		SendMail(subject, body, recipients, null);
	}
	
	public void SendMail(string subject, string body, string recipients, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			_ISN_SendMail(subject, body, recipients, "");
			#endif
		} else {
			
			
			#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_ISN_SendMail(subject, body, recipients, bytesString);
			#endif
		}
	}


	public static event Action<TextMessageComposeResult> OnTextMessageResult = delegate {};


	public void SendTextMessage(string body, string recepient, Action<TextMessageComposeResult> callback) {
		List<string> recepients = new List<string> ();
		recepients.Add (recepient);
		SendTextMessage (body, recepients, callback);
	}

	public void SendTextMessage(string body, List<string> recepients, Action<TextMessageComposeResult> callback) {
		OnTextMessageResult += callback;


		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		_ISN_SendTextMessage(body, SA.Common.Data.Converter.SerializeArray (recepients));
		#endif

	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnTextMessageComposeResult(string data) {
		int code = System.Convert.ToInt32(data);

		OnTextMessageResult ((TextMessageComposeResult)code);
		OnTextMessageResult = delegate {};

	}

	private void OnTwitterPostFailed() {
		SA.Common.Models.Result result = new SA.Common.Models.Result(new SA.Common.Models.Error());
		OnTwitterPostResult(result);
	}

	private void OnTwitterPostSuccess() {
		SA.Common.Models.Result result = new SA.Common.Models.Result();
		OnTwitterPostResult(result);
	}

	private void OnFacebookPostFailed() {
		SA.Common.Models.Result result = new SA.Common.Models.Result(new SA.Common.Models.Error());
		OnFacebookPostResult(result);
	}
	
	private void OnFacebookPostSuccess() {
		SA.Common.Models.Result result = new SA.Common.Models.Result();
		OnFacebookPostResult(result);
	}

	private void OnMailFailed() {
		SA.Common.Models.Result result = new SA.Common.Models.Result(new SA.Common.Models.Error());
		OnMailResult(result);
	}

	private void OnMailSuccess() {
		SA.Common.Models.Result result = new SA.Common.Models.Result();
		OnMailResult(result);
	}


	private void OnInstaPostSuccess() {
		SA.Common.Models.Result result = new SA.Common.Models.Result();
		OnInstagramPostResult(result);
	}
	
	
	private void OnInstaPostFailed(string data) {

		int code = System.Convert.ToInt32(data);

		SA.Common.Models.Error error =  new SA.Common.Models.Error(code, "Posting Failed");
		SA.Common.Models.Result result = new SA.Common.Models.Result(error);
		OnInstagramPostResult(result);

	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
