using UnityEngine;

namespace GameDatabase.Model
{
    public class AudioClipData
    {
        private readonly byte[] bytes;
        private bool gotValue;
        private AudioClip audioClip;
        public AudioClipData(byte[] data)
        {
            bytes = data;
            gotValue = bytes == null;
            audioClip = null;
        }

        public AudioClip AudioClip
        {
            get
            {
                if (gotValue) return audioClip;
                audioClip = OpenWavParser.ByteArrayToAudioClip(bytes);
                gotValue = true;
                return audioClip;
            }
        }

        public static AudioClipData Empty = new AudioClipData(null);
    }
}
