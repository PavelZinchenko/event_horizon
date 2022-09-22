using System.Collections.Generic;
using GameModel;
using UnityEngine;
using Utils;
using Zenject;

namespace Galaxy
{
    public class StarMap
    {
        [Inject] private readonly StarData _starData;

        public Star GetStarById(int starId)
        {
            return new Star(starId, _starData);
        }

        public float Distance(int star1, int star2)
        {
            return Vector2.Distance(_starData.GetPosition(star1), _starData.GetPosition(star2));
        }

        public IEnumerable<Star> GetVisibleStars(Vector2 topLeft, Vector2 bottomRight)
        {
            var stars = new List<Star>();

            var y0 = Mathf.FloorToInt(topLeft.y / StarLayout.DistanceY);
            var y1 = Mathf.CeilToInt(bottomRight.y / StarLayout.DistanceY);

            for (var i = y0; i <= y1; ++i)
            {
                var offset = (i % 2 == 0) ? 0.0f : 0.5f;
                var x0 = Mathf.FloorToInt((topLeft.x - offset) / StarLayout.DistanceX);
                var x1 = Mathf.CeilToInt((bottomRight.x - offset) / StarLayout.DistanceX);

                for (var j = x0; j <= x1; ++j)
                {
                    var id = StarLayout.PositionToId(j, i);
                    stars.Add(new Star(id, _starData));
                }
            }

            return stars;
        }

        public bool ShowBosses { get; set; }
        public bool ShowStores { get; set; }
        public bool ShowBookmarks { get; set; }
        public bool ShowArenas { get; set; }
        public bool ShowXmas { get; set; }

        public int GetNearestVisited(int starId, bool shoulBeSafe = false)
        {
            var center = _starData.GetPosition(starId);
            for (var i = 1; i < 10; ++i)
            {
                int id = -1;
                float min = float.MaxValue;
                foreach (var item in StarLayout.GetAdjacentStars(starId, i))
                {
                    var star = new Star(item, _starData);

                    if (!star.IsVisited)
                        continue;
                    if (shoulBeSafe && star.Occupant.CanBeAggressive)
                        continue;

                    var distance = Vector2.Distance(center, star.Position);
                    if (distance < min)
                    {
                        min = distance;
                        id = item;
                    }
                }

                if (id >= 0) return id;
            }

            OptimizedDebug.Log("max iterations reached");
            return -1;
        }

        public IEnumerable<Star> GetGalaxyViewVisibleStars(Vector2 topLeft, Vector2 bottomRight)
        {
            var stars = new List<Star>();

            var y0 = Mathf.FloorToInt(topLeft.y / StarLayout.DistanceY);
            var y1 = Mathf.CeilToInt(bottomRight.y / StarLayout.DistanceY);

            for (var i = y0; i <= y1; ++i)
            {
                var offset = (i % 2 == 0) ? 0.0f : 0.5f;
                var x0 = Mathf.FloorToInt((topLeft.x - offset) / StarLayout.DistanceX);
                var x1 = Mathf.CeilToInt((bottomRight.x - offset) / StarLayout.DistanceX);

                for (var j = x0; j <= x1; ++j)
                {
                    var id = StarLayout.PositionToId(j, i);

                    if (ShouldBeVisibleOnGalaxyMap(id))
                        stars.Add(new Star(id, _starData));
                }
            }

            return stars;
        }

        public bool ShouldBeVisibleOnGalaxyMap(int starId)
        {
            if (_starData.IsQuestObjective(starId))
                return true;
            if (!_starData.IsVisited(starId))
                return false;
            if (ShowBookmarks && _starData.HasBookmark(starId))
                return true;
            if (_starData.HasStarBase(starId))
                return true;

            var objects = _starData.GetObjects(starId);
            if (ShowStores && objects.Contain(StarObjectType.BlackMarket))
                return true;
            if (ShowArenas && objects.Contain(StarObjectType.Arena))
                return true;
            if (ShowBosses && objects.Contain(StarObjectType.Boss) && !_starData.GetBoss(starId).IsDefeated)
                return true;
            if (ShowXmas && objects.Contain(StarObjectType.Xmas))
                return true;

            return false;
        }
    }
}
