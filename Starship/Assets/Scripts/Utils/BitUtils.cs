using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class BitUtils
    {
        public static byte[] Chunks(this BitArray bits, byte chunkSize)
        {
            var length = (int)Math.Ceiling(1f * bits.Length / chunkSize);
            var data = new byte[length + 1];
            var last = length - 1;
            for (var i = 0; i < last; i++)
            {
                byte chunk = 0;
                for (var j = 0; j < chunkSize; j++)
                {
                    chunk += (byte)(Convert.ToByte(bits[i * chunkSize + j]) << j);
                }

                data[i] = chunk;
            }

            {
                byte chunk = 0;
                for (var j = 0; j < chunkSize; j++)
                {
                    var i = last * chunkSize + j;
                    if (i >= bits.Length) break;
                    chunk += (byte)(Convert.ToByte(bits[i]) << j);
                }

                data[last] = chunk;
            }

            data[data.Length - 1] = chunkSize;

            return data;
        }

        public static byte[] FitIntoLength(this BitArray bits, int maxLength)
        {
            var size = (int)Math.Ceiling(1f * bits.Length / (maxLength - 1));
            if (size > 7) throw new Exception("Data is too large");
            return bits.Chunks((byte)size);
        }

        public static BitArray FromChunks(byte[] chunks)
        {
            var chunksCount = chunks.Length - 1;
            var chunkSize = chunks[chunksCount];
            var bitsCount = (chunks.Length - 1) * chunkSize;

            var array = new BitArray(bitsCount);
            for (var i = 0; i < chunksCount; i++)
            {
                for (var j = 0; j < chunkSize; j++)
                {
                    array[i * chunkSize + j] = (chunks[i] & (1 << j)) != 0;
                }
            }

            return array;
        }

        public static void WriteIntoAlpha(this BitArray bits, Texture2D texture)
        {
            var pixels = texture.GetPixels32();
            var maxLength = pixels.Length - 4;
            var dataBytes = bits.FitIntoLength(maxLength);
            var everyNth = pixels.Length / dataBytes.Length;
            var intBytes = BitConverter.GetBytes(everyNth);
            var skip = intBytes.Length;
            for (var i = 0; i < skip; i++)
            {
                var pixel = pixels[i];
                pixel.a = (byte)(255 - intBytes[i]);
                pixels[i] = pixel;
            }

            for (var i = skip; i < pixels.Length; i += everyNth)
            {
                var pixel = pixels[i];
                var index = i / everyNth;
                if (index >= dataBytes.Length) break;
                pixel.a = (byte)(255 - dataBytes[index]);
                pixels[i] = pixel;
            }
            
            texture.SetPixels32(pixels);
        }
    }
}
