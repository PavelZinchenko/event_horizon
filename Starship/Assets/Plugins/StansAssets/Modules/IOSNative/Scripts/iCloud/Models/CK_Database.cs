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

public class CK_Database {


	public event Action<CK_RecordResult> ActionRecordSaved = delegate {};
	public event Action<CK_RecordResult> ActionRecordFetchComplete = delegate {};

	public event Action<CK_RecordDeleteResult> ActionRecordDeleted = delegate {};
	public event Action<CK_QueryResult> ActionQueryComplete = delegate {};


	private static Dictionary<int, CK_Database> _Databases =  new Dictionary<int, CK_Database>();
	private int _InternalId;

	public CK_Database(int internalId) {
		_InternalId = internalId;
		_Databases.Add(_InternalId, this);
	}


	/// <summary>
	/// Saves one record asynchronously to the current database.
	/// 
	/// This method saves the record only if it has never been saved before or if it is newer than the version on the server. 
	/// You cannot use this method to overwrite newer versions of a record on the server.
	/// </summary>
	/// <param name="record">The record object you attempted to save.</param>
	public void SaveRecrod(CK_Record record) {
		record.UpdateRecord();
		ISN_CloudKit.SaveRecord(_InternalId, record.Internal_Id);
	}


	/// <summary>
	/// Fetches one record asynchronously from the current database.
	/// 
	/// Use this method to fetch records that are not urgent to your app’s execution. 
	/// This method fetches the record with a low priority, which may cause the task to execute after higher-priority tasks.
	/// </summary>
	/// <param name="recordId">The ID of the record you want to fetch.</param>
	public void FetchRecordWithID(CK_RecordID recordId) {
		ISN_CloudKit.FetchRecord(_InternalId, recordId.Internal_Id);
	}


	/// <summary>
	/// Deletes the specified record asynchronously from the current database.
	/// 
	/// Deleting a record may trigger additional deletions if the record was referenced by other records. 
	/// This method reports only the ID of the record you asked to delete. 
	/// CloudKit does not report deletions triggered by owning relationships between records.
	/// </summary>
	/// <param name="recordId">The ID of the record you want to delete.</param>
	public void DeleteRecordWithID(CK_RecordID recordId) {
		ISN_CloudKit.DeleteRecord(_InternalId, recordId.Internal_Id);
	}



	/// <summary>
	/// Searches the specified zone asynchronously for records that match the query parameters.
	///
	/// Do not use this method when the number of returned records is potentially more than a few hundred records. 
	/// For efficiency, all queries automatically limit the number of returned records based on current conditions. 
	/// If your query hits the maximum value, this method returns only the first portion of the overall results. 
	/// The number of returned records should be sufficient in most cases.
	/// </summary>
	/// <param name="query">The ID of the record you want to delete.</param>
	public void PerformQuery(CK_Query query) {
		ISN_CloudKit.PerformQuery(_InternalId, query.Predicate, query.RecordType);
	}


	//--------------------------------------
	// Internal Use Only
	//--------------------------------------

	public int InternalId {
		get {
			return _InternalId;
		}
	}

	public static CK_Database GetDatabaseByInternalId(int id) {
		return _Databases[id];
	}


	public void FireSaveRecordResult(CK_RecordResult result) {
		result.SetDatabase(this);
		ActionRecordSaved(result);
	}

	public void FireFetchRecordResult(CK_RecordResult result) {
		result.SetDatabase(this);
		ActionRecordFetchComplete(result);
	}

	public void FireDeleteRecordResult(CK_RecordDeleteResult result) {
		result.SetDatabase(this);
		ActionRecordDeleted(result);
	}

	public void FireQueryCompleteResult(CK_QueryResult result) {
		result.SetDatabase(this);
		ActionQueryComplete(result);
	}

}