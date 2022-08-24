////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import "ISN_NativeCore.h"
#import <CloudKit/CloudKit.h>



NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_SPLITTER2 = @"|%|";
NSString * const UNITY_EOF = @"endofline";


@interface ISN_CloudKit : NSObject

@property (nonatomic, strong) CKContainer*  defaultContainer;


@property (nonatomic, strong) NSMutableDictionary* CKRecordID_Dict;
@property (nonatomic, strong) NSMutableDictionary* CKRecord_Dict;

@property (nonatomic, strong) NSMutableDictionary* CKDatabase_Dict;



+ (ISN_CloudKit *)sharedInstance;

-(void) createRecordId: (int) recordId name: (NSString*) name;
-(void) updateRecord: (int) ID type:(NSString*) type keys: (NSArray*) keys  values: (NSArray*) values recordId: (CKRecordID*) recordId;



-(void) saveRecord: (int) dbId recordId: (int) recordId;
-(void) deleteRecord: (int) dbId recordId: (int) recordId;
-(void) fetchRecord: (int) dbId recordId: (int) recordId;
-(void) performQuery:(int) dbId queryString: (NSString*)queryString recordType: (NSString*) recordType;


@end



static ISN_CloudKit * cloudkit_sharedInstance;


@implementation ISN_CloudKit

+ (id)sharedInstance {
    if (cloudkit_sharedInstance == nil)  {
        cloudkit_sharedInstance = [[self alloc] init];
    }
    
    return cloudkit_sharedInstance;
}

- (id)init {
    if ((self = [super init])) {
        
        [self setDefaultContainer:[CKContainer defaultContainer]];
        
        [self setCKDatabase_Dict:[[NSMutableDictionary alloc] init]];
        [self setCKRecord_Dict:[[NSMutableDictionary alloc] init]];
        [self setCKRecordID_Dict:[[NSMutableDictionary alloc] init]];
        
        
        NSNumber *publicDBKey = [NSNumber numberWithInt:-1];
        [[self CKDatabase_Dict] setObject:[[self defaultContainer] publicCloudDatabase] forKey:publicDBKey];
        
        NSNumber *privateDBKey = [NSNumber numberWithInt:-2];
        [[self CKDatabase_Dict] setObject:[[self defaultContainer] privateCloudDatabase] forKey:privateDBKey];
        
    }
    
    return self;
}

-(void) createRecordId:(int)recordId name:(NSString *)name {
    CKRecordID *recordID = [[CKRecordID alloc] initWithRecordName:name];
    NSNumber *key = [NSNumber numberWithInt:recordId];
    
    [[self CKRecordID_Dict] setObject:recordID forKey:key];
}

-(void) updateRecord:(int)ID type:(NSString *)type keys:(NSArray *)keys values:(NSArray *)values recordId:(CKRecordID *)recordId {
    
    NSNumber *key = [NSNumber numberWithInt:ID];
    CKRecord *record = [[CKRecord alloc] initWithRecordType:type recordID:recordId];
    
    int index = 0;
    for (NSString* recordKey in keys) {
        record[recordKey] = values[index];
        
        index++;
    }
    
    [[self CKRecord_Dict] setObject:record forKey:key];
}



-(void) saveRecord: (int) dbId recordId: (int) recordId {
    NSNumber *db_key = [NSNumber numberWithInt:dbId];
    NSNumber *record_key = [NSNumber numberWithInt:recordId];
    
    CKDatabase* database = [[self CKDatabase_Dict] objectForKey:db_key];
    CKRecord* record = [[self CKRecord_Dict] objectForKey:record_key];
    
    [database saveRecord:record completionHandler:^(CKRecord * _Nullable record, NSError * _Nullable error) {
        if(!error) {
            
            NSMutableArray * data = [[NSMutableArray alloc] init];
            [data addObject:[NSString stringWithFormat:@"%d", dbId]];
            [data addObject:[NSString stringWithFormat:@"%d", recordId]];
            
            
            UnitySendMessage("ISN_CloudKit", "OnSaveRecordSuccess", [ISN_DataConvertor NSStringsArrayToChar:data]);
        } else  {
            //record already exists
            if(error.code == 14) {
                [self OverrideRecord:dbId recordId:recordId];
            } else {
                NSMutableString * data = [[NSMutableString alloc] init];
                
                NSString* serializedData = [ISN_DataConvertor serializeErrorToNSString:error];
                
                [data appendString:[NSString stringWithFormat:@"%d", dbId]];
                [data appendString:UNITY_SPLITTER2];
                [data appendString:serializedData];
                
                UnitySendMessage("ISN_CloudKit", "OnSaveRecordFailed", [ISN_DataConvertor NSStringToChar:data]);
            }
        }
    }];
    
}

-(void) OverrideRecord: (int) dbId recordId: (int) recordId {
    
    NSNumber *db_key = [NSNumber numberWithInt:dbId];
    NSNumber *record_key = [NSNumber numberWithInt:recordId];
    
    CKDatabase* database = [[self CKDatabase_Dict] objectForKey:db_key];
    CKRecord* record = [[self CKRecord_Dict] objectForKey:record_key];
    
    NSArray<CKRecord *> * saves = [[NSArray alloc] initWithObjects:record, nil];
    CKModifyRecordsOperation *mro = [[CKModifyRecordsOperation alloc] initWithRecordsToSave:saves recordIDsToDelete:nullptr];
    
    
    mro.savePolicy = CKRecordSaveAllKeys;
    mro.qualityOfService = NSQualityOfServiceUserInitiated;
    mro.modifyRecordsCompletionBlock = ^(NSArray * _Nullable savedRecords, NSArray * _Nullable deletedRecordIDs, NSError * _Nullable error) {
        if (!error)
        {
            NSMutableArray * data = [[NSMutableArray alloc] init];
            [data addObject:[NSString stringWithFormat:@"%d", dbId]];
            [data addObject:[NSString stringWithFormat:@"%d", recordId]];
            
            UnitySendMessage("ISN_CloudKit", "OnSaveRecordSuccess", [ISN_DataConvertor NSStringsArrayToChar:data]);
        } else {
            NSMutableString * data = [[NSMutableString alloc] init];
            
            NSString* serializedData = [ISN_DataConvertor serializeErrorToNSString:error];
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:serializedData];
            
            UnitySendMessage("ISN_CloudKit", "OnSaveRecordFailed", [ISN_DataConvertor NSStringToChar:data]);
        }
    };
    [database addOperation:mro];
}


-(void) deleteRecord:(int)dbId recordId:(int)recordId {
    NSNumber *db_key = [NSNumber numberWithInt:dbId];
    NSNumber *record_key = [NSNumber numberWithInt:recordId];
    
    CKDatabase* database = [[self CKDatabase_Dict] objectForKey:db_key];
    CKRecordID* r_Id = [[self CKRecordID_Dict] objectForKey:record_key];
    
    [database deleteRecordWithID:r_Id completionHandler:^(CKRecordID * _Nullable recordID, NSError * _Nullable error) {
        if(!error) {
            
            NSMutableArray * data = [[NSMutableArray alloc] init];
            [data addObject:[NSString stringWithFormat:@"%d", dbId]];
            [data addObject:[NSString stringWithFormat:@"%d", recordId]];
            
            
            UnitySendMessage("ISN_CloudKit", "OnDeleteRecordSuccess", [ISN_DataConvertor NSStringsArrayToChar:data]);
        } else  {
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            NSString* serializedData = [ISN_DataConvertor serializeErrorToNSString:error];
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:serializedData];
            
            UnitySendMessage("ISN_CloudKit", "OnDeleteRecordFailed", [ISN_DataConvertor NSStringToChar:data]);
        }
    }];
}


-(void) performQuery:(int) dbId queryString: (NSString*)queryString recordType: (NSString*) recordType {
    
    NSNumber *db_key = [NSNumber numberWithInt:dbId];
    CKDatabase* database = [[self CKDatabase_Dict] objectForKey:db_key];
    
    
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:queryString];
    
    CKQuery* query = [[CKQuery alloc] initWithRecordType:recordType predicate:predicate];
    
    
    [database performQuery:query inZoneWithID:[CKRecordZone defaultRecordZone].zoneID completionHandler:^(NSArray<CKRecord *> * _Nullable results, NSError * _Nullable error) {
        
        if(!error) {
            
            if(results == nil) {
                results = [[NSArray alloc] init];
            }
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            
            
            for(CKRecord* r : results) {
                [data appendString:r.recordID.recordName];
                [data appendString:UNITY_SPLITTER2];
                
                [data appendString:[self serializeRecord:r]];
                [data appendString:UNITY_SPLITTER2];
                
            }
            
            [data appendString:UNITY_EOF];
            UnitySendMessage("ISN_CloudKit", "OnPerformQuerySuccess", [ISN_DataConvertor NSStringToChar:data]);
            
            
        } else {
            NSMutableString * data = [[NSMutableString alloc] init];
            
            NSString* serializedData = [ISN_DataConvertor serializeErrorToNSString:error];
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:serializedData];
            
            UnitySendMessage("ISN_CloudKit", "OnPerformQueryFailed", [ISN_DataConvertor NSStringToChar:data]);
        }
        
    }];
    
    
}


-(void) fetchRecord:(int)dbId recordId:(int)recordId {
    NSNumber *db_key = [NSNumber numberWithInt:dbId];
    NSNumber *record_key = [NSNumber numberWithInt:recordId];
    
    CKDatabase* database = [[self CKDatabase_Dict] objectForKey:db_key];
    CKRecordID* wellKnownID = [[self CKRecordID_Dict] objectForKey:record_key];
    
    [database fetchRecordWithID:wellKnownID completionHandler:^(CKRecord * _Nullable record, NSError * _Nullable error) {
        if(!error) {
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            
            [data appendString:record.recordID.recordName];
            [data appendString:UNITY_SPLITTER2];
            
            
            [data appendString:[self serializeRecord:record]];
            
            
            
            UnitySendMessage("ISN_CloudKit", "OnFetchRecordSuccess", [ISN_DataConvertor NSStringToChar:data]);
        } else  {
            
            NSMutableString * data = [[NSMutableString alloc] init];
            
            NSString* serializedData = [ISN_DataConvertor serializeErrorToNSString:error];
            
            [data appendString:[NSString stringWithFormat:@"%d", dbId]];
            [data appendString:UNITY_SPLITTER2];
            [data appendString:serializedData];
            
            UnitySendMessage("ISN_CloudKit", "OnFetchRecordFailed", [ISN_DataConvertor NSStringToChar:data]);
        }
        
    }];
}

-(NSString*) serializeRecord:(CKRecord*) record {
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString:record.recordType];
    [data appendString:UNITY_SPLITTER];
    for(NSString* key in record.allKeys) {
        [data appendString:key];
        [data appendString:UNITY_SPLITTER];
        
        NSString* val = record[key];
        
        [data appendString:val];
        [data appendString:UNITY_SPLITTER];
    }
    
    [data appendString:UNITY_EOF];
    
    return data;
}
@end

extern "C" {
    
    void _ISN_CreateRecordId_Object(int recordId, char* charName) {
        
        NSString *name = [ISN_DataConvertor charToNSString:charName];
        
        [[ISN_CloudKit sharedInstance] createRecordId:recordId name:name];
    }
    
    void _ISN_UpdateRecord_Object(int ID, char* charType, char* keys, char* values, int recordId) {
        
        NSString *type = [ISN_DataConvertor charToNSString:charType];
        NSArray* keysArray = [ISN_DataConvertor charToNSArray:keys];
        NSArray* valuesArray = [ISN_DataConvertor charToNSArray:values];
        
        NSNumber *r_key = [NSNumber numberWithInt:recordId];
        CKRecordID * recId = [[[ISN_CloudKit sharedInstance] CKRecordID_Dict] objectForKey:r_key];
        
        [[ISN_CloudKit sharedInstance] updateRecord:ID type:type keys:keysArray values:valuesArray recordId:recId];
        
    }
    
    void _ISN_SaveRecord(int dbId, int recordId) {
        [[ISN_CloudKit sharedInstance] saveRecord:dbId recordId:recordId];
    }
    
    void _ISN_PerformQuery(int dbId, char* query, char* type) {
        NSString *queryString = [ISN_DataConvertor charToNSString:query];
        NSString *recordType = [ISN_DataConvertor charToNSString:type];
        
        [[ISN_CloudKit sharedInstance] performQuery:dbId queryString:queryString recordType:recordType];
        
        
    }
    
    void _ISN_DeleteRecord(int dbId, int recordId) {
        [[ISN_CloudKit sharedInstance] deleteRecord:dbId recordId:recordId];
    }
    
    void _ISN_FetchRecord(int dbId, int recordId) {
        [[ISN_CloudKit sharedInstance] fetchRecord:dbId recordId:recordId];
    }
    
    
}
