using System;
using System.Diagnostics;
using Object = UnityEngine.Object;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
using Debug = UnityEngine.Debug;
#endif

namespace Utils
{
    /// <summary>
    /// Version of Debug logger that is completely disabled when compiling for non-debug builds
    /// </summary>
    public class OptimizedDebug
    {
        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log(message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogFormat(format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(Object context, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogFormat(context, format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogError(message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogError(message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogErrorFormat(format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogErrorFormat(context, format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogException(exception);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning(exception, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning(message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarningFormat(format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarningFormat(context, format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void ClearDeveloperConsole()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.ClearDeveloperConsole();
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Assert(condition, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Assert(condition, message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Assert(condition, message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Assert(condition, message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Assert(condition, message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.AssertFormat(condition, format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.AssertFormat(condition, context, format, args);
#endif
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogAssertion(message);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message, Object context)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogAssertion(message, context);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogAssertionFormat(format, args);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogAssertionFormat(context, format, args);
#endif
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Break()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Break();
#endif
        }
    }
}
