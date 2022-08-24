using Combat.Collision;
using GameDatabase.Model;
using Services.Audio;

namespace Combat.Component.Bullet.Action
{
    public class PlaySoundAction : IAction
    {
        public PlaySoundAction(ISoundPlayer soundPlayer, AudioClipId audioClipId, ConditionType condition)
        {
            _soundPlayer = soundPlayer;
            _audioClipId = audioClipId;
            _condition = condition;
        }

        public ConditionType Condition { get { return _condition; } }

        public CollisionEffect Invoke()
        {
            Play();
            return CollisionEffect.None;
        }

        public void Dispose()
        {
            if (_audioClipId.Loop)
                _soundPlayer.Stop(GetHashCode());
        }

        private void Play()
        {
            _soundPlayer.Play(_audioClipId, GetHashCode());
        }

        private readonly ConditionType _condition;
        private readonly ISoundPlayer _soundPlayer;
        private readonly AudioClipId _audioClipId;
    }
}
