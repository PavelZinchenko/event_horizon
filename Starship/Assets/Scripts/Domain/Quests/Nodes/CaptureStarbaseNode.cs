using GameDatabase.Enums;
using GameServices.Quests;

namespace Domain.Quests
{
    public class CaptureStarBaseNode : ActionNode
    {
        public CaptureStarBaseNode(int id, int starId, bool capture)
            : base(id, capture ? NodeType.CaptureStarBase : NodeType.LiberateStarBase)
        {
            _starId = starId;
            _capture = capture;
        }

        protected override void InvokeAction(IQuestActionProcessor processor)
        {
            processor.CaptureStarBase(_starId, _capture);
        }

        private readonly bool _capture;
        private readonly int _starId;
    }
}