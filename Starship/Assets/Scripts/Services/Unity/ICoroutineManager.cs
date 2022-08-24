using System.Collections;

namespace Services.Unity
{
    public interface ICoroutineManager
    {
        UnityEngine.Coroutine StartCoroutine(IEnumerator coroutine);
        void StartActionOnNextUpdate(System.Action<string> action, string param);
        void StartActionOnNextUpdate(System.Action<string, string> action, string param1, string param2);
    }
}
