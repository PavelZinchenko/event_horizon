////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SA.Common.Data {

	public class Converter  {

		//--------------------------------------
		// Constants
		//--------------------------------------
		
		public const char DATA_SPLITTER = '|';
		public const string DATA_SPLITTER2 = "|%|";


		public const string ARRAY_SPLITTER = "%%%";
		public const string DATA_EOF = "endofline";
		

		public static string SerializeArray(List<string> array, string splitter = ARRAY_SPLITTER) {
			return SerializeArray (array.ToArray(), splitter);
		}

		public static string SerializeArray(string[] array, string splitter = ARRAY_SPLITTER) {
			
			if(array == null) {
				return string.Empty;
			} else {
				if(array.Length == 0) {
					return string.Empty;
				} else {
					
					string serializedArray = "";
					int len = array.Length;
					for(int i = 0; i < len; i++) {
						if(i != 0) {
							serializedArray += splitter;
						}
						
						serializedArray += array[i];
					}
					
					return serializedArray;
				}
			}
		}
		
		public static string[] ParseArray(string arrayData, string splitter = ARRAY_SPLITTER) {
			
			List<string> ParsedArray =  new List<string>();
			string[] DataArray = arrayData.Split(new string[] { splitter }, StringSplitOptions.None);
		
			
			for(int i = 0; i < DataArray.Length; i ++ ) {
				if(DataArray[i] == DATA_EOF) {
					break;
				}
				ParsedArray.Add(DataArray[i]);
			}
			
			return ParsedArray.ToArray();
		}
	}

}
