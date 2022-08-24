using GameDatabase;
using GameServices.Gui;

namespace Diagnostics
{
    public interface IDebugManager
    {
        IDebugLog CreateLog(string prefix);
    }

    public class DebugManager : IDebugManager
    {
        public DebugManager(IDatabase database, ShowDebugMessageSignal.Trigger debugMessageTrigger)
        {
            _database = database;
            _debugMessageTrigger = debugMessageTrigger;
        }

        public IDebugLog CreateLog(string prefix)
        {
//#if !UNITY_EDITOR
            if (string.IsNullOrEmpty(_database.Id))
                return _empty;
//#endif

            return new DebugLog(_debugMessageTrigger, prefix);
        }

        private readonly ShowDebugMessageSignal.Trigger _debugMessageTrigger;
        private readonly IDatabase _database;
        private readonly IDebugLog _empty = new DebugLogStub();
    }
}
