using System.Collections.Generic;

namespace Gui.Utils
{
    /// <summary>
    /// Extension methods for working with weak list
    ///
    /// Notice how we are purging dead references any time we do a loop, this helps
    /// up to save extra loops exclusively for purging
    /// </summary>
    public static class WeakListExtensions
    {
        /// <summary>
        /// Removes all garbage collected elements from the list
        /// </summary>
        /// <param name="list">List of weak references to clean up</param>
        public static void Purge<T>(this IList<WeakReference<T>> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive) continue;
                list.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Iterates over all alive elements, removing dead ones in a process
        /// Note that 
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Alive<T>(this IList<WeakReference<T>> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive) yield return list[i].Target;
                else
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
