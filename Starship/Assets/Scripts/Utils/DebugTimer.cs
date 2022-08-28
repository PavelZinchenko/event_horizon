#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;

#endif

namespace Utils
{
    public static class DebugTimer
    {
        public static bool IsDebugMode = true;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static readonly long StartTime = DateTime.UtcNow.Ticks;

        private static readonly Dictionary<string, Tuple<List<double>, Stopwatch>> Timers =
            new Dictionary<string, Tuple<List<double>, Stopwatch>>();

        private static string Prepare(string message)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            stringBuilder.Append(Thread.CurrentThread.ManagedThreadId);
            stringBuilder.Append(']');
            stringBuilder.Append('[');
            stringBuilder.Append(1000L * (DateTime.UtcNow.Ticks - StartTime) / 10000000L);
            stringBuilder.Append(']');
            stringBuilder.Append(' ');
            stringBuilder.Append(message);
            return stringBuilder.ToString();
        }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // ReSharper disable Unity.PerformanceAnalysis
        private static void Log(string message)
        {
            Debug.Log(Prepare(message));
        }
#endif

        public static void CTic(string key)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Clear(key);
            Tic(key);
#endif
        }

        public static void Tic(string key)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!Timers.ContainsKey(key))
            {
                var w = new Stopwatch();
                w.Start();
                Timers[key] = Tuple.Create(new List<double>(), w);
                Log($"Started timer {key}");
            }
            else
            {
                Toc(key);
                Timers[key].Item2.Restart();
            }
#endif
        }

        public static void Toc(string key)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var (item1, s) = Timers[key];
            if (!s.IsRunning) return;
            s.Stop();
            item1.Add(1000.0 * s.ElapsedTicks / Stopwatch.Frequency);
#endif
        }

        public static void Bam(string key)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Toc(key);
            var list = Timers[key].Item1;
            var s = list.Sum();
            var c = list.Count;
            Timers.Remove(key);
            Log($"Timer {key} took {s}ms in total over {c} runs, {(c == 0 ? 0 : s / c)} on average");
#endif
        }

        public static void Clear(string key)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Timers.Remove(key);
#endif
        }


        public static void ClearConsole()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }
    }
}
