using GameDatabase.Model;
using UnityEngine;

namespace Services.Audio
{
	public interface ISoundPlayer
	{
		float Volume { get; set; }
		void Play(AudioClip audioClip, int soundId = 0, bool loop = false);
        void Play(AudioClipId audioClip, int soundId = 0);
        void Stop(int soundId);
	}
}
