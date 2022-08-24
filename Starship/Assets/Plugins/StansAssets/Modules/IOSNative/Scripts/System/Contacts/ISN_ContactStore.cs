//#define CONTACTS_API_ENABLED
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
#if (UNITY_IPHONE && !UNITY_EDITOR && CONTACTS_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


namespace SA.IOSNative.Contacts {


	public class ContactStore : SA.Common.Pattern.Singleton<ContactStore> {

		#if (UNITY_IPHONE && !UNITY_EDITOR && CONTACTS_API_ENABLED) || SA_DEBUG_MODE
		[DllImport ("__Internal")]
		private static extern void _ISN_RetrievePhoneContacts();


		[DllImport ("__Internal")]
		private static extern void ISN_ShowContactsPicker();



		#endif

		private event Action<ContactsResult> ContactsLoadResult = delegate {};
		private event Action<ContactsResult> ContactsPickResult = delegate {};



		//--------------------------------------
		// Initialization
		//--------------------------------------

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}


		//--------------------------------------
		// Public Methods
		//--------------------------------------


		public void ShowContactsPickerUI(Action<ContactsResult> callback) {
			ContactsPickResult += callback;

			#if (UNITY_IPHONE && !UNITY_EDITOR && CONTACTS_API_ENABLED) || SA_DEBUG_MODE
			ISN_ShowContactsPicker();
			#endif
		}


		public void RetrievePhoneContacts(Action<ContactsResult> callback) {
			ContactsLoadResult += callback;

			#if (UNITY_IPHONE && !UNITY_EDITOR && CONTACTS_API_ENABLED) || SA_DEBUG_MODE
			 _ISN_RetrievePhoneContacts();
			#endif
		}



		//--------------------------------------
		// Private Methods
		//--------------------------------------


		private Contact ParseContactData(string data) {
			string[] DataArray = data.Split(SA.Common.Data.Converter.DATA_SPLITTER); 

			Contact contact =  new Contact();

			contact.GivenName 	= DataArray[0];
			contact.FamilyName 	= DataArray[1];

			string[] emails =  SA.Common.Data.Converter.ParseArray(DataArray[2]);
			contact.Emails.AddRange(emails);

			string[] countryCodes =  SA.Common.Data.Converter.ParseArray(DataArray[3]);
			string[] digits =  SA.Common.Data.Converter.ParseArray(DataArray[4]);

			for(int i = 0; i < countryCodes.Length; i++) {
				PhoneNumber number =  new PhoneNumber();
				number.CountryCode 	= countryCodes[i];
				number.Digits 		= digits[i];

				contact.PhoneNumbers.Add(number);

			}

			return contact;

		}


		private List<Contact> ParseContactArray(string data) {
			string[] DataArray = data.Split(new string[] { SA.Common.Data.Converter.DATA_SPLITTER2 }, StringSplitOptions.None);

			List<Contact> contacts =  new List<Contact>();
			for (int i = 0; i < DataArray.Length; i++) {
				if (DataArray[i] == SA.Common.Data.Converter.DATA_EOF) {
					break;
				}

				Contact contact = ParseContactData(DataArray[i]);
				contacts.Add(contact);
			}

			return contacts;

		}


		//--------------------------------------
		// Native Events
		//--------------------------------------


		private void OnContactPickerDidCancel(string errorData) {
			var error =  new SA.Common.Models.Error(0, "User Canceled");

			ContactsResult result =  new ContactsResult(error);
			ContactsPickResult(result);
			ContactsPickResult = delegate {};
		}


		private void OnPickerDidSelectContacts(string data) {
			ISN_Logger.Log("[ContactStore] OnPickerDidSelectContacts");
			List<Contact> contacts = ParseContactArray (data);
			ISN_Logger.Log("[ContactStore] Picked " + contacts.Count + " contacts");


			ContactsResult result =  new ContactsResult(contacts);
			ContactsPickResult(result);
			ContactsPickResult = delegate {};
		}



		private void OnContactsRetrieveFailed(string errorData) {

			ISN_Logger.Log("[ContactStore] OnContactsRetrieveFailed");

			var error =  new SA.Common.Models.Error(errorData);

			ContactsResult result =  new ContactsResult(error);
			ContactsLoadResult(result);
			ContactsLoadResult = delegate {};
			
		}


		private void OnContactsRetrieveFinished(string data) {

			ISN_Logger.Log("[ContactStore] OnContactsRetrieveFinished");


			List<Contact> contacts = ParseContactArray (data);
			ISN_Logger.Log("[ContactStore] Loaded " + contacts.Count + " contacts");


			ContactsResult result =  new ContactsResult(contacts);
			ContactsLoadResult(result);
			ContactsLoadResult = delegate {};
		}



	

	}

}


