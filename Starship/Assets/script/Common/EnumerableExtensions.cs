using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtension
{
    public static T MinValue<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> predicate)
        where TResult : IComparable
    {
        TResult minValue = default(TResult);
        T currentItem = default(T);
        bool inited = false;
        foreach (var item in collection)
        {
            var value = predicate(item);
            if (!inited || value.CompareTo(minValue) < 0)
            {
                inited = true;
                minValue = value;
                currentItem = item;
            }
        }

        return currentItem;
    }

    public static int FindIndex<T>(this IList<T> source, Predicate<T> match)
    {
        var count = source.Count;
        for (int i = 0; i < count; i++)
        {
            if (match(source[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T value)
    {
        foreach (var item in enumerable)
            yield return item;

        yield return value;
    }

    public static IEnumerable<T> Concat<T>(this T value, IEnumerable<T> enumerable)
    {
        yield return value;

        foreach (var item in enumerable)
            yield return item;
    }

    public static bool Empty<T>(this IList<T> list)
    {
        return list.Count == 0;
    }

    public static IList<T> Shuffle<T>(this IList<T> list, Random random)
    {
        var n = list.Count - 1;
        while (n > 1)
        {
            var k = random.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }

    public static IList<T> ShuffledCopy<T>(this IList<T> list, Random random)
    {
        return list.ToArray().Shuffle(random);
    }

    public static IEnumerable<T> OddElements<T>(this IEnumerable<T> collection)
    {
        int index = 0;
        foreach (var item in collection)
            if ((++index & 1) == 0)
                yield return item;
    }

    public static T RandomElement<T>(this IEnumerable<T> collection, Random random)
    {
        var enumerable = collection as IList<T> ?? collection.ToArray();
        var count = enumerable.Count;
        return count > 0 ? enumerable[random.Next(count)] : default;
    }

    /// <summary>
    /// Fetches random elements from a collection.
    /// </summary>
    /// <param name="collection">collection to extract elements from</param>
    /// <param name="count">amount of elements to extract</param>
    /// <param name="random">random numbers generator to be used</param>
    public static IList<T> RandomElements<T>(this IEnumerable<T> collection, int count, Random random)
    {
        return (collection as IList<T> ?? collection.ToArray()).RandomElements(count, random);
    }

    /// <summary>
    /// Fetches random elements from a list.
    /// </summary>
    /// <param name="list">list to extract elements from</param>
    /// <param name="count">amount of elements to extract</param>
    /// <param name="random">random numbers generator to be used</param>
    public static IList<T> RandomElements<T>(this IList<T> list, int count, Random random)
    {
        var data = new T[count];
        var size = list.Count;

        for (var i = 0; i < count; i++)
        {
            data[i] = list[random.Next(size)];
        }

        return data;
    }

    /// <summary>
    /// Fetches random unique elements from a collection.
    /// <br/>
    /// If requested items count is larger than requested items count, shuffled collection will get returned.
    /// </summary>
    /// <param name="collection">collection to extract elements from</param>
    /// <param name="count">amount of elements to extract</param>
    /// <param name="random">random numbers generator to be used</param>
    public static IList<T> RandomUniqueElements<T>(this IEnumerable<T> collection, int count,
        Random random)
    {
        if (collection is IList<T> list) return list.RandomUniqueElements(count, random);
        return collection.ToArray().RandomUniqueElements(count, random, true);
    }

    /// <summary>
    /// Fetches random unique elements from a list.
    /// If requested items count is larger than requested items count, shuffled list will get returned.
    /// </summary>
    /// <param name="list">list to extract elements from</param>
    /// <param name="count">amount of elements to extract</param>
    /// <param name="random">random numbers generator to be used</param>
    /// <param name="shuffleInPlace">specifies if input list should be sorted itself instead of creating a copy in case
    /// if list items count is less than requested items count</param>
    public static IList<T> RandomUniqueElements<T>(this IList<T> list, int count, Random random,
        bool shuffleInPlace = false)
    {
        if (list.Count <= count) return shuffleInPlace ? list.Shuffle(random) : list.ShuffledCopy(random);

        var arr = new T[count];

        var itemsLeft = count;
        var max = list.Count;
        var size = max;

        for (var i = 0; i < max; ++i)
        {
            if (itemsLeft <= 0)
                return arr;

            if (random.Next(size) < itemsLeft)
            {
                arr[count - itemsLeft] = list[i];
                itemsLeft--;
            }

            size--;
        }

        return arr.Shuffle(random);
    }

    public static IList<int> RandomUniqueNumbers(int min, int max, int count, Random random)
    {
        var arr = new int[Math.Min(max - min + 1, count)];
        if (count >= max - min)
        {
            for (var i = min; i <= max; ++i)
            {
                arr[i - min] = i;
            }

            return arr;
        }

        var itemsLeft = count;
        var size = max - min + 1;

        for (var i = min; i <= max; ++i)
        {
            if (itemsLeft <= 0)
                return arr;

            if (random.Next(size) < itemsLeft)
            {
                arr[count - itemsLeft] = i;
                itemsLeft--;
            }

            size--;
        }

        return arr;
    }

    /// <summary>
    /// Checks if amount of elements matching the predicate is withing [min,max] bounds (inclusive)
    /// </summary>
    /// <param name="collection">Collection to run predicate on</param>
    /// <param name="predicate">testing function</param>
    /// <param name="min">minimal required number of elements</param>
    /// <param name="max">maximum allowed value</param>
    public static bool CountIsBetween<T>(this IEnumerable<T> collection, Func<T, bool> predicate, int min, int max)
    {
        if (min > max) (min, max) = (max, min);
        var count = 0;
        foreach (var item in collection)
        {
            if (predicate(item)) count++;
            if (count >= max) return false;
        }

        return count >= min;
    }
    
    /// <summary>
    /// Checks if amount of elements matching the predicate is at least min
    /// </summary>
    /// <param name="collection">Collection to run predicate on</param>
    /// <param name="predicate">testing function</param>
    /// <param name="min">required number of elements</param>
    public static bool CountIsAtLeast<T>(this IEnumerable<T> collection, Func<T, bool> predicate, int min)
    {
        var count = 0;
        foreach (var item in collection)
        {
            if (predicate(item)) count++;
            if (count >= min) return true;
        }

        return false;
    }

    public static IEnumerable<T> ToEnumerable<T>(this T item)
    {
        return new[] { item };
    }

    public static T[] Add<T>(this T[] target, T item)
    {
        if (target == null)
        {
            return new[] { item };
        }

        var result = new T[target.Length + 1];
        target.CopyTo(result, 0);
        result[target.Length] = item;
        return result;
    }

    public static T[] AddIfNotExists<T>(this T[] target, T item)
    {
        if (target == null)
        {
            return new[] { item };
        }

        if (Array.IndexOf(target, item) >= 0)
            return target;

        var result = new T[target.Length + 1];
        target.CopyTo(result, 0);
        result[target.Length] = item;
        return result;
    }

    public static T[] RemoveAll<T>(this T[] target, T item) where T : class
    {
        if (target == null)
            return null;

        var index = 0;
        for (var i = 0; i < target.Length; ++i)
        {
            target[index] = target[i];
            if (target[i] != item)
                index++;
        }

        if (index == 0)
            return new T[] { };

        var result = new T[index];
        Array.Copy(target, result, index);
        return result;
    }

    public static void ReplaceAll<T>(this IList<T> list, IEnumerable<T> newValues)
    {
        list.Clear();
        foreach (var value in newValues)
            list.Add(value);
    }

    public static void SetValue<T>(this IList<T> list, T value, int index)
    {
        while (list.Count <= index)
            list.Add(default(T));
        list[index] = value;
    }
}
