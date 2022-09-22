using System;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Player;
using Services.InternetTime;
using Session;
using Session.Content;
using Utils;
using Zenject;

namespace Domain.Quests
{
    public class QuestFactory
    {
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly FleetFactory _fleetFactory;
        [Inject] private readonly Loot.Factory _lootFactory;
        [Inject] private readonly GameTime _gameTime;

        public Quest Create(QuestData.QuestProgress progress)
        {
            var data = _database.GetQuest(new ItemId<QuestModel>(progress.QuestId));
            if (data == null)
            {
                OptimizedDebug.LogException(new ArgumentException("QuestFactory: quest not found - " + progress.QuestId));
                return null;
            }

            var builder = new QuestBuilder(data, progress.StarId, progress.Seed, _starData, _motherShip, _session, _lootFactory, _fleetFactory, _gameTime);
            return builder.Build(progress.ActiveNode);
        }

        public Quest Create(QuestModel data, int starId, int seed)
        {
            var builder = new QuestBuilder(data, starId, seed, _starData, _motherShip, _session, _lootFactory, _fleetFactory, _gameTime);
            return builder.Build();
        }
    }
}
