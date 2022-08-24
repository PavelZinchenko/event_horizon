using System;
using UnityEngine;
using GameModel.Session;

//public class Game : MonoBehaviour
//{
//    //[SerializeField] GameModel.GameFlow _gameFlow;
//    //[SerializeField] LevelManager _levelManager;
    
//    //public static ISession Session { get { throw new NotImplementedException(); } }
//    //public static ILevelManager Level { get { return Instance._levelManager; } }

//    //public static bool Paused { get { return Instance._gameFlow.Paused; } set { Instance._gameFlow.Paused = value; } }

// //   public void CreateNewSession(Services.Storage.IDataStorage dataStorage = null)
// //   {
// //       if (Instance._session != null)
// //           ServiceLocator.Messenger.Broadcast(EventType.SessionBeforeClose);

// //       var gamedata = GameDataBase.TryLoad(dataStorage) ?? new GameDataBase();
// //       gamedata.MergePurchases(Session.GameData);
// //       var newSession = new GameModel.Session.Session(gamedata);
// //       //Instance._session = newSession;
// //       newSession.Start();
// //       ServiceLocator.Messenger.Broadcast(EventType.SessionCreated);
// //   }

//	//public void RestorePurchases()
//	//{
//	//	ServiceLocator.InAppPurchasing.RestorePurchases();
//	//	var storedData = ServiceLocator.IapStorage.Read();
//	//	var iapData = Session.GameData.Purchases;

//	//	iapData.RemoveAds |= storedData.RemoveAds;
//	//	iapData.SupporterPack |= storedData.SupporterPack;

//	//	if (storedData.PurchasedStars > iapData.PurchasedStars) 
//	//	{
//	//		var extraStars = storedData.PurchasedStars - iapData.PurchasedStars;
//	//		iapData.PurchasedStars = storedData.PurchasedStars;
//	//		Session.GameData.Resources.Stars += extraStars;
//	//	}
//	//}

//#region UNITY

//    private void Awake()
//    {
//        DontDestroyOnLoad(gameObject);
//        Instance = this;
//    }

//    //private void Start()
//    //{
//    //    LoadSession();
//    //}

//    //private void Update()
//    //{
//    //    if (_session != null)
//    //        _session.Update(Time.deltaTime);
//    //}

//#endregion

//    //private void LoadSession()
//    //{
//    //    if (_session != null)
//    //        ServiceLocator.Messenger.Broadcast(EventType.SessionBeforeClose);
//    //    var gameData = GameDataBase.TryLoad(ServiceLocator.LocalDataStorage) ?? GameDataBase.TryLoadLegacyGame() ?? new GameDataBase();
//    //    _session = new GameModel.Session.Session(gameData);
//    //    _session.Start();
//    //    ServiceLocator.Messenger.Broadcast(EventType.SessionCreated);
//    //}

//    //private Session _session;
//    private static Game Instance;
//}
