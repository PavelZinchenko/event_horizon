using System.Collections.Generic;
using System.Text;
using Services.Localization;

namespace Domain.Quests
{
    public class BooleanRequirements : IRequirements
    {
        public enum Operation
        {
            All,
            Any,
            None,
        }

        public BooleanRequirements(Operation operation)
        {
            _operation = operation;
        }

        public void Add(IRequirements requirements)
        {
            _requirements.Add(requirements);
        }

        public bool IsMet
        {
            get
            {
                if (_requirements.Count == 0)
                    return _operation == Operation.None;

                foreach (var item in _requirements)
                {
                    var isMet = item.IsMet;
                    if (isMet && _operation == Operation.Any)
                        return true;
                    if (isMet && _operation == Operation.None)
                        return false;
                    if (!isMet && _operation == Operation.All)
                        return false;
                }

                return true;
            }
        }

        public bool CanStart(int starId, int seed)
        {
            if (_requirements.Count == 0)
                return _operation == Operation.None;

            foreach (var item in _requirements)
            {
                var isMet = item.CanStart(starId, seed);
                if (isMet && _operation == Operation.Any)
                    return true;
                if (isMet && _operation == Operation.None)
                    return false;
                if (!isMet && _operation == Operation.All)
                    return false;
            }

            return true;
        }

        public string GetDescription(ILocalization localization)
        {
            if (_requirements.Count == 0)
                return string.Empty;

            if (_operation == Operation.All)
            {
                foreach (var item in _requirements)
                {
                    if (item.IsMet) continue;
                    var description = item.GetDescription(localization);
                    if (string.IsNullOrEmpty(description)) continue;
                    return description;
                }
            }

            return string.Empty;
        }

        public int BeaconPosition
        {
            get
            {
                foreach (var item in _requirements)
                {
                    var position = item.BeaconPosition;
                    if (position >= 0)
                        return position;
                }

                return -1;
            }
        }

        private readonly Operation _operation;
        private readonly List<IRequirements> _requirements = new List<IRequirements>();
    }
}
