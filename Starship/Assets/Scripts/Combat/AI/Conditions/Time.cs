namespace Combat.Ai.Condition
{
    public class LessTimePassedCondition : ICondition
    {
        public LessTimePassedCondition(State<float> startTime, float interval)
        {
            _startTime = startTime;
            _interval = interval;
        }

        public bool IsTrue(Context context)
        {
            var time = context.CurrentTime - _startTime.Value;
            return time < _interval;
        }

        private readonly State<float> _startTime;
        private readonly float _interval;
    }

    public class MoreTimePassedCondition : ICondition
    {
        public MoreTimePassedCondition(State<float> startTime, float interval)
        {
            _startTime = startTime;
            _interval = interval;
        }

        public bool IsTrue(Context context)
        {
            return context.CurrentTime - _startTime.Value > _interval;
        }

        private readonly State<float> _startTime;
        private readonly float _interval;
    }
}
