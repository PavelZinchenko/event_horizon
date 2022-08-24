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

public class ISN_Locale  {


	//Returns the country code for this locale, or "" if this locale doesn't correspond to a specific country.
	private string _CountryCode;
	
	//Returns the name of this locale's country, localized to locale. Returns the empty string if this locale does not correspond to a specific country.
	private string _DisplayCountry;
	
	
	//Returns the language code for this Locale or the empty string if no language was set.
	private string _LanguageCode;
	
	//Returns the name of this locale's language, localized to locale. If the language name is unknown, the language code is returned.
	private string _DisplayLanguage;



	public ISN_Locale(string countryCode, string contryName, string languageCode, string languageName) {
		_CountryCode = countryCode;
		_DisplayCountry = contryName;
		_LanguageCode = languageCode;
		_DisplayLanguage = languageName;
	}

	public string CountryCode {
		get {
			return _CountryCode;
		}
	}

	public string DisplayCountry {
		get {
			return _DisplayCountry;
		}
	}

	public string LanguageCode {
		get {
			return _LanguageCode;
		}
	}

	public string DisplayLanguage {
		get {
			return _DisplayLanguage;
		}
	}
}
