using GameModel;
using UnityEngine;

namespace Galaxy
{
    public struct Star
    {
		public Star(int id, StarData starData)
        {
            _id = id;
			_starData = starData;
        }

		public int Id => _id;
        public int Level => _starData.GetLevel(_id);
        public bool IsVisited => _starData.IsVisited(_id);
        public void SetVisited() { _starData.SetVisited(_id); }
        public Region Region => _starData.GetRegion(_id);
        public Vector2 Position => _starData.GetPosition(_id);
        public string Bookmark 
        {
            get => _starData.GetBookmark(_id);
            set => _starData.SetBookmark(_id, value);
        }
        public string Name => _starData.GetName(_id);
        public StarObjects Objects => _starData.GetObjects(_id);
        public bool HasStarBase => _starData.HasStarBase(_id);
        public bool HasBookmark => _starData.HasBookmark(_id);
        public void CaptureBase() { _starData.CaptureBase(_id); }
        public bool IsQuestObjective => _starData.IsQuestObjective(_id);

        public StarContent.Occupants.Facade Occupant => _starData.GetOccupant(_id);
        public StarContent.Boss.Facade Boss => _starData.GetBoss(_id);
        public StarContent.Ruins.Facade Ruins => _starData.GetRuins(_id);
        public StarContent.XmasTree.Facade Xmas => _starData.GetXmasTree(_id);
        public StarContent.Challenge.Facade Challenge => _starData.GetChallenge(_id);
        public StarContent.LocalEvent.Facade LocalEvent => _starData.GetLocalEvent(_id);
        public StarContent.Survival.Facade Survival => _starData.GetSurvival(_id);
        public StarContent.Wormhole.Facade Wormhole => _starData.GetWormhole(_id);
        public StarContent.Hive.Facade Pandemic => _starData.GetPandemic(_id);

        private readonly int _id;
		private readonly StarData _starData;
    }
}
