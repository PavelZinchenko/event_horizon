using System.Collections.Generic;

namespace Gui.Utils
{
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
    }
}
