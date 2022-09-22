using System;
using System.Runtime.Serialization;
using System.Threading;
using GameServices;
using Utils;
using Zenject;

namespace Services.Diagnostics
{
    public class FreezeDetector : IInitializable, IDisposable, ITickable
    {
        [Inject]
        public FreezeDetector(GamePausedSignal gamePausedSignal, GameFlow gameFlow)
        {
            _gamePausedSignal = gamePausedSignal;
            _gamePausedSignal.Event += OnGamePaused;
            _gameFlow = gameFlow;
        }

        public void Initialize()
        {
            _mainThread = Thread.CurrentThread;

            var thread = new Thread(ThreadFunc);
            while (thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                OptimizedDebug.Log("Invalid thread id (" + thread.ManagedThreadId + ")");
                thread = new Thread(ThreadFunc);
            }

            thread.Priority = ThreadPriority.Normal;
            thread.Start(this);
        }

        public void Dispose()
        {
            _terminate = true;
        }

        public void Tick()
        {
            UpdateTime();
        }

        public bool Paused { get; private set; }

        private void OnGamePaused(bool value)
        {
            var paused = _gameFlow.ApplicationPaused;

            OptimizedDebug.Log("FreezeDetector: OnGamePaused(" + paused + ")");
            UpdateTime();
            Paused = paused;
        }

        private void UpdateTime()
        {
            Interlocked.Exchange(ref _lastTickTime, DateTime.UtcNow.Ticks);
        }

        private void ThreadFunc(object data)
        {
            var context = (FreezeDetector)data;
            OptimizedDebug.Log("FreezeDetector thread started");

            while (!context._terminate)
            {
                Thread.Sleep(5000);

                if (context.Paused)
                    continue;

                var time = Interlocked.Read(ref context._lastTickTime);
                var deltatime = time > 0 ? (double)(DateTime.UtcNow.Ticks - time)/TimeSpan.TicksPerSecond : 0.0;
                OptimizedDebug.Log("Freeze detector: " + deltatime);

                if (deltatime > 30)
                {
                    OptimizedDebug.LogError("Freeze detected");
#if !UNITY_EDITOR
                    context._mainThread.Abort();
#else
                    OptimizedDebug.Break();
#endif
                    //break;
                }
            }
        }

        private Thread _mainThread;
        private bool _terminate = false;
        private long _lastTickTime = 0;
        private readonly GameFlow _gameFlow;
        private readonly GamePausedSignal _gamePausedSignal;
    }
}
