using UnityEngine;
using System.Collections.Generic;

/*
 * This static class is developped to convert a wav file (from a byte array) into a
 * UnityEngine AudioClip to be played dynamically.
 * 
 * The other function allows your application to pick any AudioClip with valid data
 * and convert its content into a wav file (to a byte array).
 * 
 * The way you read or save the files is up to you.
 * 
 * eToile recommends using FileManagement to read and save files (OWP is already integrated):
 * https://assetstore.unity.com/packages/tools/user-tools/input-management/file-management-easy-way-to-save-and-read-files-67183
 */

public static class OpenWavParser
{
    /// <summary>Load audio file into AudioClip</summary>
    public static AudioClip ByteArrayToAudioClip(byte[] wavFile, string name = "", bool stream = false)
    {
        /* WAV file format:
         * 
         * size - Name              - (index)   Description.
         * 
         * 4    - ChunkID           - (0)       "RIFF"
         * 4    - ChunkSize         - (4)       file size minus 8 (RIFF(4) + ChunkSize(4)).
         * 4    - Format            - (8)       "WAVE"
         * 
         * 4    - Subchunk1ID       - (12)      "fmt "
         * 4    - Subchunk1Size     - (16)      16 for PCM (20 to 36)
         * 2    - AudioFormat       - (20)      1 for PCM (other values implies some compression).
         * 2    - NumChannels       - (22)      Mono = 1, Stereo = 2, etc.
         * 4    - SampleRate        - (24)      8000, 22050, 44100, etc.
         * 4    - ByteRate          - (28)      == SampleRate * NumChannels * (BitsPerSample/8)
         * 2    - BlockAlign        - (32)      == NumChannels * (BitsPerSample/8)
         * 2    - BitsPerSample     - (34)      8 bits = 8, 16 bits = 16, etc.
         * (Here goes the extra data pointed by Subchunk1Size > 16)
         * 
         * 4    - Subchunk2ID       - (36)      "data"
         * 4    - Subchunk2Size     - (40)
         * Subchunk2Size (Data)     - (44)
         */
        
        //int _chunkSize = System.BitConverter.ToInt32(wavFile, 4);     // Not used.
        int _subchunk1Size = System.BitConverter.ToInt32(wavFile, 16);
        int _audioFormat = System.BitConverter.ToInt16(wavFile, 20);
        int _numChannels = System.BitConverter.ToInt16(wavFile, 22);
        int _sampleRate = System.BitConverter.ToInt32(wavFile, 24);
        //int _byteRate = System.BitConverter.ToInt32(wavFile, 28);     // Not used.
        //int _blockAlign = System.BitConverter.ToInt16(wavFile, 32);   // Not used.
        int _bitsPerSample = System.BitConverter.ToInt16(wavFile, 34);
        // Find where data starts:
        int _dataIndex = 20 + _subchunk1Size;
        for (int i = _dataIndex; i < wavFile.Length; i++)
        {
            if (wavFile[i] == 'd' && wavFile[i + 1] == 'a' && wavFile[i + 2] == 't' && wavFile[i + 3] == 'a')
            {
                _dataIndex = i + 4;     // "data" string size = 4
                break;
            }
        }
        // Data parameters:
        int _subchunk2Size = System.BitConverter.ToInt32(wavFile, _dataIndex);  // Data size (Subchunk2Size).
        _dataIndex += 4;                                                        // Subchunk2Size = 4
        int _sampleSize = _bitsPerSample / 8;                                   // Size of a sample.
        int _sampleCount = _subchunk2Size / _sampleSize;                        // How many samples into data.
        // WAV method:
        if (_audioFormat == 1)
        {
            float[] _audioBuffer = new float[_sampleCount];  // Size for all available channels.
            for (int i = 0; i < _sampleCount; i++)
            {
                var sampleIndex = _dataIndex + i * _sampleSize;

                if (_sampleSize == 1)
                    _audioBuffer[i] = wavFile[sampleIndex] / 255f;
                else
                    _audioBuffer[i] = System.BitConverter.ToInt16(wavFile, sampleIndex) / 32768f;
            }
            // Create the AudioClip:
            AudioClip audioClip = AudioClip.Create(name, _sampleCount, _numChannels, _sampleRate, stream);
            audioClip.SetData(_audioBuffer, 0);
            return audioClip;
        }
        else
        {
            Debug.LogError("[OpenWavParser.ByteArrayToAudioClip] Compressed wav format not supported.");
            return null;
        }
    }

    // Returns a wav file from any AudioClip (doesn't saves):
    public static byte[] AudioClipToByteArray(AudioClip clip)
    {
        // Clip content:
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);                                                       // The audio data in samples.

        // Write all data to byte array:
        List<byte> wavFile = new List<byte>();
        
        // RIFF header:
        wavFile.AddRange(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });    // "RIFF"
        wavFile.AddRange(System.BitConverter.GetBytes(samples.Length * 2 + 44 - 8));    // ChunkSize
        wavFile.AddRange(new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });    // "WAVE"
        wavFile.AddRange(new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });    // "fmt"
        wavFile.AddRange(System.BitConverter.GetBytes(16));                             // Subchunk1Size (16bit for PCM)
        wavFile.AddRange(System.BitConverter.GetBytes((ushort)1));                      // AudioFormat (1 for PCM)
        wavFile.AddRange(System.BitConverter.GetBytes((ushort)clip.channels));          // NumChannels
        wavFile.AddRange(System.BitConverter.GetBytes(clip.frequency));                 // SampleRate
        wavFile.AddRange(System.BitConverter.GetBytes(clip.frequency * clip.channels * (16/8)));    // ByteRate
        wavFile.AddRange(System.BitConverter.GetBytes((ushort)(clip.channels * (16 / 8))));         // BlockAlign
        wavFile.AddRange(System.BitConverter.GetBytes((ushort)16));                                 // BitsPerSample
        wavFile.AddRange(new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });    // "data"
        wavFile.AddRange(System.BitConverter.GetBytes(samples.Length * 2));             // Subchunk2Size
        // Add the audio data in bytes:
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * 32768.0F);
            wavFile.AddRange(System.BitConverter.GetBytes(sample));                     // The audio data in bytes.
        }
        // Return the byte array to be saved:
        return wavFile.ToArray();
    }
}