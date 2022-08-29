using System.Collections.Generic;

namespace Gui.Utils
{
    public static class WeakListExtensions
    {
        public static void RetainAlive<T>(this IList<WeakReference<T>> list)
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
