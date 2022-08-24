using GameDatabase.Model;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class SoundEffect : IUnitEffect
    {
        public SoundEffect(ISoundPlayer soundPlayer, AudioClipId audioClipId, ConditionType playCondition, ConditionType stopCondition = ConditionType.None)
        {
            _soundPlayer = soundPlayer;
            _audioClipId = audioClipId;
            _playCondition = playCondition;
            _stopCondition = stopCondition;
        }

        public ConditionType TriggerCondition { get { return _playCondition | _stopCondition; } }
        public bool TryUpdateEffect(float elapsedTime) { return false; }

        public bool TryInvokeEffect(ConditionType condition)
        {
            if (condition.Contains(_stopCondition))
                Stop();
            else if (condition.Contains(_playCondition))
                Play();
            return false;
        }

        public void Dispose()
        {
            Stop();
        }

        private void Play()
        {
            if (_isPlaying)
                return;

            _soundPlayer.Play(_audioClipId, GetHashCode());
            _isPlaying = _audioClipId.Loop;
        }

        private void Stop()
        {
            if (!_isPlaying)
                return;

            _soundPlayer.Stop(GetHashCode());
            _isPlaying = false;
        }

        private bool _isPlaying;
        private readonly ConditionType _playCondition;
        private readonly ConditionType _stopCondition;
        private readonly ISoundPlayer _soundPlayer;
        private readonly AudioClipId _audioClipId;
    }
}
