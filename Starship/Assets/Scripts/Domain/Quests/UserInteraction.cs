using System.Collections.Generic;
using GameDatabase.Enums;
using GameDatabase.Model;
using Model.Military;

namespace Domain.Quests
{
    public enum Severity
    {
        Danger,
        Warning,
        Info,
    }

    public interface IUserInteraction
    {
        string Message { get; }
        string CharacterName { get; set; }
        SpriteId CharacterAvatar { get; set; }
        RequiredViewMode RequiredView { get; }
        IEnumerable<UserAction> Actions { get; }
        IFleet Enemies { get; }
        ILoot Loot { get; }
    }

    public class UserAction
    {
        public UserAction(IQuestEventData data, string text, INodeRequirements requirements, Severity severity/*, bool enabled*/)
        {
            _data = data;
            Text = text;
            Requirements = requirements;
            Severity = severity;
            //Enabled = enabled;
        }

        public readonly string Text;
        public readonly INodeRequirements Requirements;
        public readonly Severity Severity;
        //public readonly bool Enabled;

        public void Invoke(QuestEventSignal.Trigger questEventTrigger)
        {
            questEventTrigger.Fire(_data);
        }

        private readonly IQuestEventData _data;
    }

    public class UserInteraction : IUserInteraction
    {
        public string Message { get; set; }
        public string CharacterName { get; set; }
        public SpriteId CharacterAvatar { get; set; }
        public RequiredViewMode RequiredView { get; set; }
        public IEnumerable<UserAction> Actions { get; set; }
        public ILoot Loot { get; set; }
        public IFleet Enemies { get; set; }
    }
}
