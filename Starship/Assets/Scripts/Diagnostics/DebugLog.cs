using GameServices.Gui;

namespace Diagnostics
{
    public class DebugLog : IDebugLog
    {
        public DebugLog(ShowDebugMessageSignal.Trigger showMessageTrigger, string prefix = null)
        {
            _showMessageTrigger = showMessageTrigger;
            _prefix = prefix;
        }

        public void Write(string message)
        {
            _showMessageTrigger.Fire(string.IsNullOrEmpty(_prefix) ? message : string.Format("[{0}] {1}", _prefix, message));
        }

        private readonly string _prefix;
        private readonly ShowDebugMessageSignal.Trigger _showMessageTrigger;
    }
}
