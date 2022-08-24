using UnityEngine;

namespace GameDatabase.Storage
{
    public class UnityJsonSerializer : IJsonSerializer
    {
        public T FromJson<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }

        public string ToJson<T>(T item)
        {
            return JsonUtility.ToJson(item, true).Replace("    ", "  ");
        }
    }
}
