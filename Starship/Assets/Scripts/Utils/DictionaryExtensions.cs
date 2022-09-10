using System.Collections.Generic;

namespace Utils
{
    public static class DictionaryExtensions
    {
        public static void Increment<T>(this IDictionary<T,int> dict, T item, int amount=1)
        {
            if (!dict.TryGetValue(item, out var result)) dict[item] = amount;
            else dict[item] = result + amount;
        }

        public static V Get<K,V>(this IDictionary<K,V> dict, K key, V fallback)
        {
            return dict.TryGetValue(key, out var result) ? result : fallback;
        }
    }
}
