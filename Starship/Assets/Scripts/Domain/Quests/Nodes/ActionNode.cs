using System.Collections.Generic;
using Domain.Quests;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

public abstract class ActionNode : INode
{
    protected ActionNode(int id, NodeType type)
    {
        _id = id;
        _type = type;
    }

    public int Id { get { return _id; } }
    public NodeType Type { get { return _type; } }
    public INode TargetNode { get; set; }

    public string GetRequirementsText(ILocalization localization)
    {
//#if UNITY_EDITOR
//        return Type + " - " + _id;
//#else
        return string.Empty;
//#endif
    }

    public bool TryGetBeacons(ICollection<int> beacons) { return false; }
    public virtual void Initialize() { ActionRequired = true; }

    public bool TryProceed(out INode target)
    {
        if (ActionRequired)
        {
            target = this;
            return false;
        }

        target = TargetNode;
        return true;
    }

    public virtual bool TryProcessEvent(IQuestEventData data, out INode target)
    {
        target = this;
        return false;
    }

    public bool ActionRequired { get; private set; }

    public bool TryInvokeAction(IQuestActionProcessor processor)
    {
        if (!ActionRequired) return false;
        ActionRequired = false;
        InvokeAction(processor);
        return true;
    }

    protected abstract void InvokeAction(IQuestActionProcessor processor);

    private readonly int _id;
    private readonly NodeType _type;
}
