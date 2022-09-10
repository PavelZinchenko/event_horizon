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
    }
}
