using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class QuestData : ISerializableData
    {
        [Inject]
        public QuestData(byte[] buffer = null)
        {
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "quests";

        public bool IsChanged { get; private set; }
        public static int CurrentVersion => 2;

        public bool HasBeenStarted(int questId)
        {
            return _statistics.ContainsKey(questId) || _progress.ContainsKey(questId);
        }

        public bool HasBeenCompleted(int questId)
        {
            return _statistics.TryGetValue(questId, out var statistics) && statistics.CompletionCount > 0;
        }

        public long LastCompletionTime(int questId)
        {
            return _statistics.TryGetValue(questId, out var statistics) ? statistics.LastCompletionTime : 0;
        }

        public long LastStartTime(int questId)
        {
            return _statistics.TryGetValue(questId, out var statistics) ? statistics.LastStartTime : 0;
        }

        public bool IsActiveOrCompleted(int questId)
        {
            if (_progress.ContainsKey(questId)) return true;
            QuestStatistics statistics;
            if (!_statistics.TryGetValue(questId, out statistics))
                return false;

            return statistics.CompletionCount > 0;
        }

        public int TotalQuestCount()
        {
            var total = 0;
            foreach (var statistic in _statistics.Values)
                total += statistic.CompletionCount + statistic.FailureCount;

            foreach (var progress in _progress.Values)
                total += progress.Count;

            return total;
        }

        public bool IsQuestActive(int questId)
        {
            return _progress.ContainsKey(questId);
        }

        public bool IsQuestActive(int questId, int starId)
        {
            Dictionary<int, QuestProgressInternal> progress;
            return _progress.TryGetValue(questId, out progress) && progress.ContainsKey(starId);
        }

        public long QuestStartTime(int questId, int starId)
        {
            Dictionary<int, QuestProgressInternal> progress;
            if (!_progress.TryGetValue(questId, out progress)) return 0;
            if (!progress.TryGetValue(starId, out var progressInternal)) return 0;
            return progressInternal.StartTime;
        }

        public QuestStatistics GetQuestStatistics(int questId)
        {
            QuestStatistics statistics;
            if (!_statistics.TryGetValue(questId, out statistics))
                statistics = new QuestStatistics();

            return statistics;
        }

        public IEnumerable<QuestProgress> GetActiveQuests()
        {
            foreach (var progress in _progress)
            {
                foreach (var data in progress.Value)
                {
                    yield return new QuestProgress
                    {
                        QuestId = progress.Key,
                        StarId = data.Key,
                        Seed = data.Value.Seed,
                        ActiveNode = data.Value.ActiveNode,
                    };
                }
            }
        }

        public IEnumerable<QuestProgress> GetQuestProgress(int questId)
        {
            Dictionary<int, QuestProgressInternal> progress;
            if (!_progress.TryGetValue(questId, out progress)) return Enumerable.Empty<QuestProgress>();

            return progress.Select(data => new QuestProgress
            {
                QuestId = questId,
                StarId = data.Key,
                Seed = data.Value.Seed,
                ActiveNode = data.Value.ActiveNode
            });
        }

        public void SetQuestProgress(int questId, int starId, int seed, int activeNode, long currentTime)
        {
            IsChanged = true;

            Dictionary<int, QuestProgressInternal> progress;
            if (!_progress.TryGetValue(questId, out progress))
            {
                progress = new Dictionary<int, QuestProgressInternal>();
                _progress[questId] = progress;
            }

            if (!progress.TryGetValue(starId, out var data))
            {
                data = new QuestProgressInternal { StartTime = currentTime };
                if (!_statistics.TryGetValue(questId, out var statistics)) statistics = new QuestStatistics();
                statistics.LastStartTime = currentTime;
                _statistics[questId] = statistics;
            }

            data.Seed = seed;
            data.ActiveNode = activeNode;
            progress[starId] = data;
        }

        public void CancelQuest(int questId, int starId)
        {
            IsChanged = true;
            Dictionary<int, QuestProgressInternal> progress;
            if (!_progress.TryGetValue(questId, out progress))
                return;

            progress.Remove(starId);
            if (progress.Count == 0) _progress.Remove(questId);
        }

        public void SetQuestCompleted(int questId, int starId, bool success, long completionTime)
        {
            IsChanged = true;

            var statistics = GetQuestStatistics(questId);

            if (success)
                statistics.CompletionCount++;
            else
                statistics.FailureCount++;

            if (completionTime > statistics.LastCompletionTime)
                statistics.LastCompletionTime = completionTime;

            _statistics[questId] = statistics;

            CancelQuest(questId, starId);
        }

        public int GetFactionRelations(int starId)
        {
            int value;
            return _factionRelations.TryGetValue(starId, out value) ? value : 0;
        }

        public void SetFactionRelations(int starId, int value)
        {
            IsChanged = true;
            _factionRelations[starId] = UnityEngine.Mathf.Clamp(value, -100, 100);
        }

        public int GetCharacterRelations(int characterId)
        {
            int value;
            return _characterRelations.TryGetValue(characterId, out value) ? value : 0;
        }

        public void SetCharacterRelations(int characterId, int value)
        {
            IsChanged = true;
            _characterRelations[characterId] = UnityEngine.Mathf.Clamp(value, -100, 100);
        }

        public void Reset()
        {
            _progress.Clear();
            _statistics.Clear();
            _factionRelations.Clear();
            _characterRelations.Clear();
        }

        public IEnumerable<byte> Serialize()
        {
            IsChanged = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;

            foreach (var value in Helpers.Serialize(_factionRelations))
                yield return value;

            foreach (var value in Helpers.Serialize(_characterRelations))
                yield return value;

            foreach (var value in Helpers.Serialize(_statistics.Count))
                yield return value;
            foreach (var stat in _statistics)
            {
                foreach (var value in Helpers.Serialize(stat.Key))
                    yield return value;
                foreach (var value in Helpers.Serialize(stat.Value.CompletionCount))
                    yield return value;
                foreach (var value in Helpers.Serialize(stat.Value.FailureCount))
                    yield return value;
                foreach (var value in Helpers.Serialize(stat.Value.LastStartTime))
                    yield return value;
                foreach (var value in Helpers.Serialize(stat.Value.LastCompletionTime))
                    yield return value;
            }

            foreach (var value in Helpers.Serialize(_progress.Count))
                yield return value;
            foreach (var progress in _progress)
            {
                foreach (var value in Helpers.Serialize(progress.Key))
                    yield return value;
                foreach (var value in Helpers.Serialize(progress.Value.Count))
                    yield return value;
                foreach (var quest in progress.Value)
                {
                    foreach (var value in Helpers.Serialize(quest.Key))
                        yield return value;
                    foreach (var value in Helpers.Serialize(quest.Value.Seed))
                        yield return value;
                    foreach (var value in Helpers.Serialize(quest.Value.ActiveNode))
                        yield return value;
                    foreach (var value in Helpers.Serialize(quest.Value.StartTime))
                        yield return value;
                }
            }
        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            int index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);
            if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
            {
                UnityEngine.Debug.Log("QuestData: incorrect data version");
                throw new ArgumentException();
            }

            _factionRelations = Helpers.DeserializeDictionary(buffer, ref index);
            _characterRelations = Helpers.DeserializeDictionary(buffer, ref index);

            _statistics.Clear();
            var count = Helpers.DeserializeInt(buffer, ref index);
            for (var i = 0; i < count; ++i)
            {
                var questId = Helpers.DeserializeInt(buffer, ref index);
                var completionCount = Helpers.DeserializeInt(buffer, ref index);
                var failureCount = Helpers.DeserializeInt(buffer, ref index);
                var lastStartTime = Helpers.DeserializeLong(buffer, ref index);
                var lastCompletionTime = Helpers.DeserializeLong(buffer, ref index);
                _statistics.Add(questId, new QuestStatistics
                {
                    CompletionCount = completionCount, 
                    FailureCount = failureCount, 
                    LastStartTime = lastStartTime,
                    LastCompletionTime = lastCompletionTime,
                });
            }

            _progress.Clear();
            count = Helpers.DeserializeInt(buffer, ref index);
            for (var i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeInt(buffer, ref index);
                var items = new Dictionary<int, QuestProgressInternal>();
                _progress.Add(key, items);

                var itemsCount = Helpers.DeserializeInt(buffer, ref index);
                for (var j = 0; j < itemsCount; ++j)
                {
                    var starId = Helpers.DeserializeInt(buffer, ref index);
                    var seed = Helpers.DeserializeInt(buffer, ref index);
                    var node = Helpers.DeserializeInt(buffer, ref index);
                    var startTime = Helpers.DeserializeLong(buffer, ref index);
                    items.Add(starId, new QuestProgressInternal { Seed = seed, ActiveNode = node, StartTime = startTime });
                }
            }

            IsChanged = false;
        }

        private static bool TryUpgrade(ref byte[] data, int version)
        {
            if (version == 1)
            {
                data = Upgrade_1_2(data).ToArray();
                version = 2;
            }

            return version == CurrentVersion;
        }

        private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index);
            var version = 2;
            foreach (var value in Helpers.Serialize(version))
                yield return value;

            var factionRelations = Helpers.DeserializeDictionary(buffer, ref index);
            var characterRelations = Helpers.DeserializeDictionary(buffer, ref index);

            foreach (var value in Helpers.Serialize(factionRelations))
                yield return value;

            foreach (var value in Helpers.Serialize(characterRelations))
                yield return value;

            var count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;
            
            for (var i = 0; i < count; ++i)
            {
                var questId = Helpers.DeserializeInt(buffer, ref index);
                var completionCount = Helpers.DeserializeInt(buffer, ref index);
                var failureCount = Helpers.DeserializeInt(buffer, ref index);

                foreach (var value in Helpers.Serialize(questId))
                    yield return value;
                foreach (var value in Helpers.Serialize(completionCount))
                    yield return value;
                foreach (var value in Helpers.Serialize(failureCount))
                    yield return value;
                foreach (var value in Helpers.Serialize(0L)) // LastStartTime
                    yield return value;
                foreach (var value in Helpers.Serialize(0L)) // LastCompletionTime
                    yield return value;
            }

            count = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(count))
                yield return value;

            for (var i = 0; i < count; ++i)
            {
                var key = Helpers.DeserializeInt(buffer, ref index);
                var itemCount = Helpers.DeserializeInt(buffer, ref index);
                //var items = new Dictionary<int, QuestProgressInternal>();
                foreach (var value in Helpers.Serialize(key))
                    yield return value;
                foreach (var value in Helpers.Serialize(itemCount))
                    yield return value;

                for (var j = 0; j < itemCount; ++j)
                {
                    var starId = Helpers.DeserializeInt(buffer, ref index);
                    var seed = Helpers.DeserializeInt(buffer, ref index);
                    var node = Helpers.DeserializeInt(buffer, ref index);

                    foreach (var value in Helpers.Serialize(starId))
                        yield return value;
                    foreach (var value in Helpers.Serialize(seed))
                        yield return value;
                    foreach (var value in Helpers.Serialize(node))
                        yield return value;
                    foreach (var value in Helpers.Serialize(0L)) // StartTime
                        yield return value;
                }
            }
        }

        private Dictionary<int, int> _factionRelations = new Dictionary<int, int>();
        private Dictionary<int, int> _characterRelations = new Dictionary<int, int>();
        private readonly Dictionary<int, QuestStatistics> _statistics = new Dictionary<int, QuestStatistics>();
        private readonly Dictionary<int, Dictionary<int, QuestProgressInternal>> _progress = new Dictionary<int, Dictionary<int, QuestProgressInternal>>();

        public struct QuestStatistics
        {
            public int CompletionCount;
            public int FailureCount;
            public long LastStartTime;
            public long LastCompletionTime;
        }

        public struct QuestProgress
        {
            public int QuestId;
            public int StarId;
            public int Seed;
            public int ActiveNode;
            public long StartTime;
        }

        private struct QuestProgressInternal
        {
            public int Seed;
            public int ActiveNode;
            public long StartTime;
        }
    }
}
