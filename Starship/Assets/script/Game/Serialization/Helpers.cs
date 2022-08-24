using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GameModel
{
	namespace Serialization
	{
		public static class Helpers
		{
			public static IEnumerable<byte> Serialize(int data)
			{
				yield return (byte)(data);
				yield return (byte)(data >> 8);
				yield return (byte)(data >> 16);
				yield return (byte)(data >> 24);
			}

            public static IEnumerable<byte> Serialize(short data)
            {
                yield return (byte)(data);
                yield return (byte)(data >> 8);
            }

			public static IEnumerable<byte> Serialize(long data)
			{
				foreach (var value in BitConverter.GetBytes(data))
					yield return value;
			}
            public static IEnumerable<byte> Serialize(ulong data)
            {
                foreach (var value in BitConverter.GetBytes(data))
                    yield return value;
            }

            public static IEnumerable<byte> Serialize(HashSet<int> data)
			{
				foreach (var value in BitConverter.GetBytes(data.Count))
					yield return value;
				foreach (var item in data)
					foreach (var value in BitConverter.GetBytes(item))
						yield return value;
			}

			public static IEnumerable<byte> Serialize(Dictionary<int, int> data)
			{
				foreach (var value in Serialize(data.Count))
					yield return value;
				foreach (var item in data)
				{
					foreach (var value in Serialize(item.Key))
						yield return value;
					foreach (var value in Serialize(item.Value))
						yield return value;
				}
			}

		    public static IEnumerable<byte> Serialize(IGameItemCollection<int> data)
		    {
		        foreach (var value in Serialize(data.Count))
		            yield return value;
		        foreach (var item in data.Items)
		        {
		            foreach (var value in Serialize(item.Key))
		                yield return value;
		            foreach (var value in Serialize(item.Value))
		                yield return value;
		        }
		    }

            public static IEnumerable<byte> Serialize(Dictionary<long, int> data)
			{
				foreach (var value in Serialize(data.Count))
					yield return value;
				foreach (var item in data)
				{
					foreach (var value in Serialize(item.Key))
						yield return value;
					foreach (var value in Serialize(item.Value))
						yield return value;
				}
			}
			
			public static IEnumerable<byte> Serialize(string data)
			{
				if (string.IsNullOrEmpty(data))
				{
					yield return 0;
					yield return 0;
					yield return 0;
					yield return 0;
					yield break;
                }
                
				var bytes = System.Text.Encoding.UTF8.GetBytes(data);

				foreach (var value in BitConverter.GetBytes(bytes.Length))
					yield return value;
				foreach (var value in System.Text.Encoding.UTF8.GetBytes(data))
					yield return value;
            }

			public static IEnumerable<byte> Serialize(float data)
			{
				foreach (var value in BitConverter.GetBytes(data))
					yield return value;
			}
			
			public static IEnumerable<byte> Serialize(Vector2 data)
			{
				foreach (var value in BitConverter.GetBytes(data.x))
					yield return value;
				foreach (var value in BitConverter.GetBytes(data.y))
					yield return value;
			}
			
			public static IEnumerable<byte> Serialize(Position data)
			{
				foreach (var value in BitConverter.GetBytes(data.x))
					yield return value;
				foreach (var value in BitConverter.GetBytes(data.y))
					yield return value;
			}

            public static IEnumerable<byte> Serialize(IEnumerable<long> data)
            {
                if (data == null)
                {
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield break;
                }

                foreach (var value in Serialize(data.Count()))
                    yield return value;
                foreach (var item in data)
                    foreach (var value in Serialize(item))
                        yield return value;
            }

            public static IEnumerable<byte> Serialize(IEnumerable<byte[]> data)
            {
                if (data == null)
                {
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield break;
                }

                foreach (var value in Serialize(data.Count()))
                    yield return value;
                foreach (var item in data)
                {
                    foreach (var value in Serialize(item.Length))
                        yield return value;
                    foreach (var value in item)
                        yield return value;
                }
            }

            public static IEnumerable<byte> Serialize(IEnumerable<int> data)
            {
                if (data == null)
                {
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield return 0;
                    yield break;
                }

                foreach (var value in Serialize(data.Count()))
                    yield return value;
                foreach (var item in data)
                    foreach (var value in Serialize(item))
                        yield return value;
            }

		    public static IEnumerable<byte> Serialize(byte[] data)
		    {
		        if (data == null || data.Length == 0)
		        {
		            yield return 0;
		            yield return 0;
		            yield return 0;
		            yield return 0;
		            yield break;
		        }

		        foreach (var value in Serialize(data.Length))
		            yield return value;
		        foreach (var value in data)
		            yield return value;
		    }

			public static IEnumerable<byte> Serialize(IEnumerable<string> data)
			{
				if (data == null)
				{
					yield return 0;
					yield return 0;
					yield return 0;
					yield return 0;
					yield break;
				}

				foreach (var value in Serialize(data.Count()))
					yield return value;
				foreach (var item in data)
					foreach (var value in Serialize(item))
						yield return value;
			}

			public static int DeserializeInt(byte[] data, ref int index)
			{
				return (int)data[index++] | (((int)data[index++]) << 8) | (((int)data[index++]) << 16) | (((int)data[index++]) << 24);
			}			

			public static float DeserializeFloat(byte[] data, ref int index)
			{
				var value = BitConverter.ToSingle(data, index);
				index += sizeof(float);
				return value;
			}

			public static short DeserializeShort(byte[] data, ref int index)
            {
                return (short)((uint)data[index++] | (uint)(data[index++] << 8));
            }

			public static long DeserializeLong(byte[] data, ref int index)
			{
				var value = BitConverter.ToInt64(data, index);
				index += sizeof(long);
                return value;
            }
 
			public static Vector2 DeserializeVector2(byte[] data, ref int index)
			{
				return new Vector2(DeserializeFloat(data, ref index), DeserializeFloat(data, ref index));
			}			

			public static Position DeserializePosition(byte[] data, ref int index)
			{
				return new Position(DeserializeFloat(data, ref index), DeserializeFloat(data, ref index));
			}			
			
			public static string DeserializeString(byte[] data, ref int index)
			{
				int count = BitConverter.ToInt32(data, index);
				index += sizeof(int);
				if (count == 0) 
					return string.Empty;
				var result = System.Text.Encoding.UTF8.GetString(data, index, count);
				index += count;
				return result;
            }
            
			public static string[] DeserializeStringArray(byte[] data, ref int index)
			{
				int count = DeserializeInt(data, ref index);
				if (count <= 0)
					return new string[] {};

				var array = new string[count];
				for (int i = 0; i < count; ++i)
					array[i] = Helpers.DeserializeString(data, ref index);

				return array;
			}
			
            public static int[] DeserializeIntArray(byte[] data, ref int index)
            {
                int count = DeserializeInt(data, ref index);
                if (count <= 0)
                    return new int[] {};

                var array = new int[count];
                for (int i = 0; i < count; ++i)
                    array[i] = Helpers.DeserializeInt(data, ref index);

                return array;
            }

		    public static long[] DeserializeLongArray(byte[] data, ref int index, bool allowNull = false)
            {
                int count = DeserializeInt(data, ref index);
                if (count <= 0)
                    return allowNull ? null : new long[] {};

                var array = new long[count];
                for (int i = 0; i < count; ++i)
                    array[i] = Helpers.DeserializeLong(data, ref index);

                return array;
            }

		    public static byte[] DeserializeByteArray(byte[] data, ref int index, bool allowNull = false)
		    {
		        var length = Helpers.DeserializeInt(data, ref index);
		        if (length <= 0)
		            return allowNull ? null : new byte[] { };

                var array = new byte[length];
		        Array.Copy(data, index, array, 0, length);
		        index += length;
		        return array;
		    }

            public static List<byte[]> DeserializeByteArrays(byte[] data, ref int index)
            {
                int count = DeserializeInt(data, ref index);
                if (count <= 0)
                    return new List<byte[]>();

                var list = new List<byte[]>(count);
                for (var i = 0; i < count; ++i)
                    list.Add(Helpers.DeserializeByteArray(data, ref index));

                return list;
            }

            public static HashSet<int> DeserializeHashSet(byte[] data, ref int index)
			{
				var result = new HashSet<int>();
				int count = BitConverter.ToInt32(data, index);
				index += sizeof(int);
				for (int i = 0; i < count; ++i)
				{
					result.Add(BitConverter.ToInt32(data, index));
					index += sizeof(int);
				}
				return result;
			}

			public static Dictionary<int, int> DeserializeDictionary(byte[] data, ref int index)
			{
				var result = new Dictionary<int,int>();
				int count = BitConverter.ToInt32(data, index);
				index += sizeof(int);
				for (int i = 0; i < count; ++i)
				{
					var key = DeserializeInt(data, ref index);
					var value = DeserializeInt(data, ref index);
					result.Add(key, value);
				}
				return result;
			}

			public static Dictionary<long, int> DeserializeDictionaryLongInt(byte[] data, ref int index)
			{
				var result = new Dictionary<long,int>();
				int count = BitConverter.ToInt32(data, index);
				index += sizeof(int);
				for (int i = 0; i < count; ++i)
				{
					var key = DeserializeLong(data, ref index);
					var value = DeserializeInt(data, ref index);
					result.Add(key, value);
				}
				return result;
			}
		}
	}
}