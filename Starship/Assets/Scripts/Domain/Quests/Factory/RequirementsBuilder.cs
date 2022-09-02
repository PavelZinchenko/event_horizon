using System;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameServices.Player;
using GameModel;
using Services.InternetTime;
using Session;

namespace Domain.Quests
{
    public class RequirementsBuilder : IRequirementFactory<IRequirements>
    {
        public RequirementsBuilder(QuestContext context, ILootCache lootCache, MotherShip motherShip, ISessionData session, GameTime gameTime)
        {
            _motherShip = motherShip;
            _context = context;
            _lootCache = lootCache;
            _session = session;
            _gameTime = gameTime;
        }

        public IRequirements Build(Requirement requirement)
        {
            return requirement.Create(this);
        }

        public IRequirements Create(Requirement_Empty content)
        {
            return EmptyRequirements.Instance;
        }

        public IRequirements Create(Requirement_Any content)
        {
            return Boolean(content.Requirements, BooleanRequirements.Operation.Any);
        }

        public IRequirements Create(Requirement_All content)
        {
            return Boolean(content.Requirements, BooleanRequirements.Operation.All);
        }

        public IRequirements Create(Requirement_None content)
        {
            return Boolean(content.Requirements, BooleanRequirements.Operation.None);
        }

        public IRequirements Create(Requirement_PlayerPosition content)
        {
            var minDistance = UnityEngine.Mathf.Clamp(content.MinValue, 0, 1000);
            var maxDistance = content.MaxValue >= minDistance ? content.MaxValue : int.MaxValue;

            return new CurrentStarRequirements(minDistance, maxDistance, _motherShip);
        }

        public IRequirements Create(Requirement_RandomStarSystem content)
        {
            var random = new System.Random(_context.Seed);
            var minDistance = UnityEngine.Mathf.Clamp(content.MinValue, 1, 1000);
            var maxDistance = UnityEngine.Mathf.Clamp(content.MaxValue, minDistance, 1000);
            var distance = maxDistance > minDistance ? minDistance + random.Next(maxDistance - minDistance + 1) : minDistance;
            var starId = StarLayout.GetAdjacentStars(_context.StarId, distance).RandomElement(random);
            return new StarRequirements(starId, _motherShip);
        }

        public IRequirements Create(Requirement_AggressiveOccupants content)
        {
            return new EnemiesWantFightRequirements(_motherShip);
        }

        public IRequirements Create(Requirement_QuestCompleted content)
        {
            return new QuestRequirement(content.Quest, QuestRequirement.RequiredStatus.Completed, _session);
        }

        public IRequirements Create(Requirement_QuestActive content)
        {
            return new QuestRequirement(content.Quest, QuestRequirement.RequiredStatus.Active, _session);
        }

        public IRequirements Create(Requirement_CharacterRelations content)
        {
            return new CharacterRelationsRequirement(content.Character, content.MinValue, content.MaxValue, _session);
        }

        public IRequirements Create(Requirement_FactionRelations content)
        {
            return new FactionReputationRequirement(_context.StarId, content.MinValue, content.MaxValue, _session);
        }

        public IRequirements Create(Requirement_StarbaseCaptured content)
        {
            return new StarbaseCapturedRequirement( _motherShip );
        }

        public IRequirements Create(Requirement_Faction content)
        {
            return new FactionRequirements(content.Faction, _motherShip);
        }

        public IRequirements Create(Requirement_HaveQuestItem content)
        {
            return new ArtifactRequirement(content.QuestItem, content.Amount, _session);
        }

        public IRequirements Create(Requirement_HaveItem content)
        {
            return new ItemsRequirements(_lootCache.Get(new LootModel(content.Loot), _context));
        }

        public IRequirements Create(Requirement_HaveItemById content)
        {
            return new ItemsRequirements(_lootCache.Get(content.Loot, _context));
        }

        public IRequirements Create(Requirement_TimeSinceQuestStart content)
        {
            return new TimeSinceQuestStart(_context.QuestId, _context.StarId, _session, TimeSpan.TicksPerMinute * (content.Hours*60 + content.Minutes), _gameTime);
        }

        public IRequirements Create(Requirement_TimeSinceLastCompletion content)
        {
            return new TimeSinceLastCompletion(_context.QuestId, _session, TimeSpan.TicksPerMinute * (content.Hours * 60 + content.Minutes), _gameTime);
        }

        public IRequirements Create(Requirement_ComeToOrigin content)
        {
            return new StarRequirements(_context.StarId, _motherShip);
        }

        private IRequirements Boolean(IEnumerable<Requirement> requirements, BooleanRequirements.Operation operation)
        {
            var result = new BooleanRequirements(operation);
            if (requirements == null)
                return result;

            foreach (var item in requirements)
                result.Add(item.Create(this));

            return result;
        }

        private readonly QuestContext _context;
        private readonly ILootCache _lootCache;
        private readonly MotherShip _motherShip;
        private readonly ISessionData _session;
        private readonly GameTime _gameTime;
    }
}
