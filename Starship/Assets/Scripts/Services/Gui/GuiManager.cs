using System;
using System.Collections.Generic;
using System.Linq;
using GameServices.LevelManager;
using Gui.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace Services.Gui
{
    public class GuiManager : IInitializable, IDisposable, ITickable, IGuiManager
    {
        [Inject]
        public GuiManager(
            SceneLoadedSignal sceneLoadedSignal,
            WindowOpenedSignal windowOpenedSignal,
            WindowClosedSignal windowClosedSignal,
            EscapeKeyPressedSignal.Trigger escapeKeyPressedTrigger)
        {
            _sceneLoadedSignal = sceneLoadedSignal;
            _windowOpenedSignal = windowOpenedSignal;
            _windowClosedSignal = windowClosedSignal;
            _escapeKeyPressedTrigger = escapeKeyPressedTrigger;

            _sceneLoadedSignal.Event += OnSceneLoaded;
            _windowOpenedSignal.Event += OnWindowOpened;
            _windowClosedSignal.Event += OnWindowClosed;
        }

        public void Initialize()
        {
            ScanSceneForWindows();
        }

        public void Dispose()
        {
            Cleanup();
        }

        public void OpenWindow(string id, Action<WindowExitCode> onCloseAction = null)
        {
            IWindow window;
            if (!_windows.TryGetValue(id, out window))
                throw new ArgumentException();

            if (onCloseAction != null)
                _onCloseActions.Add(window, onCloseAction);

            window.Open();
        }

        public void OpenWindow(string id, WindowArgs args, Action<WindowExitCode> onCloseAction = null)
        {
            IWindow window;
            if (!_windows.TryGetValue(id, out window))
                throw new ArgumentException();

            if (onCloseAction != null)
                _onCloseActions.Add(window, onCloseAction);

            window.Open(args);
        }

        public bool AutoWindowsAllowed
        {
            get { return _autoWindowsAllowed; }
            set
            {
                if (_autoWindowsAllowed == value)
                    return;

                _autoWindowsAllowed = value;
                if (_autoWindowsAllowed)
                    TryOpenShowWhenPossibleWindows();
            }
        }

        public void CloseAllWindows()
        {
            var temp = AutoWindowsAllowed;

            AutoWindowsAllowed = false;

            while (_activeWindows.Any())
                _activeWindows.First().Close();

            AutoWindowsAllowed = temp;
        }

        private void OnWindowOpened(string id)
        {
            //OptimizedDebug.Log("Window opened: " + id);

            IWindow window;
            if (!_windows.TryGetValue(id, out window))
                throw new InvalidOperationException("invalid window id - " + id);

            var windowsToClose = new List<IWindow>();
            var enabled = true;

            foreach (var item in _activeWindows)
            {
                if (window.Class.CantBeOpenedDueTo(item.Class))
                {
                    OptimizedDebug.Log("Window cant be opened: " + window.Id + " due to " + item.Id);
                    window.Close();
                    return;
                }

                if (item.Class.MustBeClosedDueTo(window.Class))
                {
                    //OptimizedDebug.Log("Window must be closed: " + item.Id + " due to " + window.Id);
                    windowsToClose.Add(item);
                    continue;
                }

                if (item.Class.MustBeDisabledDueTo(window.Class) && item.Enabled)
                {
                    //OptimizedDebug.Log("Window must be disabled: " + item.Id + " due to " + window.Id);
                    item.Enabled = false;
                }
                if (window.Class.MustBeDisabledDueTo(item.Class))
                {
                    //OptimizedDebug.Log("Window must be disabled: " + window.Id + " due to " + item.Id);
                    enabled = false;
                }
            }

            window.Enabled = enabled;
            _activeWindows.Add(window);

            foreach (var item in windowsToClose)
                item.Close();
        }

        private void OnWindowClosed(string id, WindowExitCode exitCode)
        {
            //OptimizedDebug.Log("Window closed: " + id);

            IWindow window;
            if (!_windows.TryGetValue(id, out window))
                //throw new InvalidOperationException();
                return;

            _activeWindows.Remove(window);

            UpdateActiveWindows();

            Action<WindowExitCode> action;
            if (_onCloseActions.TryGetValue(window, out action))
            {
                _onCloseActions.Remove(window);
                action.Invoke(exitCode);
            }

            if (window.Class.MustBeOpenedAutomatically() && exitCode != WindowExitCode.Ok)
                ShowWindowWhenPossible(window);

            TryOpenShowWhenPossibleWindows();
        }

        private void ShowWindowWhenPossible(IWindow window)
        {
            _showWhenPossibleWindows.RemoveWhere(item => item.Class.MustBeClosedDueTo(window.Class));
            _showWhenPossibleWindows.Add(window);
        }

        private bool CanOpenShowWhenPossibleWindow(IWindow window)
        {
            foreach (var item in _activeWindows)
            {
                if (window.Class.CantBeOpenedDueTo(item.Class))
                    return false;
                if (window.Class.MustBeClosedDueTo(item.Class))
                    return false;
            }

            return true;
        }

        private void TryOpenShowWhenPossibleWindows()
        {
            if (!AutoWindowsAllowed)
                return;

            for (var i = 0; i < 100; ++i)
            {
                var window = _showWhenPossibleWindows.FirstOrDefault(CanOpenShowWhenPossibleWindow);
                if (window == null)
                    return;

                _showWhenPossibleWindows.Remove(window);

                if (!window.IsVisible)
                    window.Open();
            }

            throw new InvalidOperationException();
        }

        private void UpdateActiveWindows()
        {
            foreach (var first in _activeWindows)
            {
                var enabled = !_activeWindows.Where(second => first != second).
                    Any(second => first.Class.MustBeDisabledDueTo(second.Class));

                if (first.Enabled != enabled)
                    first.Enabled = enabled;
            }
        }

        private void OnSceneLoaded()
        {
            ScanSceneForWindows();
        }

        private ICollection<IWindow> GetLoadedWindows()
        {
            var windows = new List<IWindow>();
            var sceneCount = SceneManager.sceneCount;

            foreach (var item in DontDestroyOnLoad.All)
                FindWindows(item.transform, windows);

            var rootObjects = new List<GameObject>();
            for (var i = 0; i < sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                rootObjects.Clear();
                scene.GetRootGameObjects(rootObjects);

                foreach (var rootObject in rootObjects)
                {
                    if (rootObject.GetComponent<DontDestroyOnLoad>() != null) continue;
                    FindWindows(rootObject.transform, windows);
                }
            }

            return windows;
        }

        private void FindWindows(Transform root, ICollection<IWindow> windows)
        {
            foreach (Transform child in root)
            {
                var item = child.GetComponent<IWindow>();
                if (item != null)
                    windows.Add(item);
                else
                    FindWindows(child, windows);
            }
        }

        private void ScanSceneForWindows()
        {
            OptimizedDebug.Log("GuiManager.ScanSceneForWindows");

            Cleanup();
            var windows = GetLoadedWindows();
            foreach (var window in windows)
            {
                if (_windows.ContainsKey(window.Id))
                {
                    OptimizedDebug.LogError("Window already exists - " + window.Id);
                    OptimizedDebug.Break();
                }

                _windows.Add(window.Id, window);

                if (window.IsVisible)
                {
                    OptimizedDebug.Log("Active window found: " + window.Id);
                    _activeWindows.Add(window);
                }
                else
                {
                    //OptimizedDebug.Log("Window found: " + window.Id);
                }
            }
        }

        private void Cleanup()
        {
            _windows.Clear();
            _activeWindows.Clear();
            _showWhenPossibleWindows.Clear();

            var actions = _onCloseActions.Values.ToArray();
            _onCloseActions.Clear();

            foreach (var item in actions)
                item.Invoke(WindowExitCode.Cancel);
        }

        private bool _autoWindowsAllowed = true;
        //private readonly HashSet<WindowClass> _deniedClasses;
        private readonly HashSet<IWindow> _showWhenPossibleWindows = new HashSet<IWindow>();
        private readonly Dictionary<string, IWindow> _windows = new Dictionary<string, IWindow>();
        private readonly HashSet<IWindow> _activeWindows = new HashSet<IWindow>();
        private readonly Dictionary<IWindow, Action<WindowExitCode>> _onCloseActions = new Dictionary<IWindow, Action<WindowExitCode>>();

        private readonly SceneLoadedSignal _sceneLoadedSignal;
        private readonly WindowOpenedSignal _windowOpenedSignal;
        private readonly WindowClosedSignal _windowClosedSignal;
        private readonly EscapeKeyPressedSignal.Trigger _escapeKeyPressedTrigger;

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IWindow windowToClose = null;
                foreach (var window in _activeWindows)
                {
                    if (window.EscapeAction == EscapeKeyAction.Block)
                        return;
                    if (window.EscapeAction == EscapeKeyAction.Close && window.Enabled)
                        windowToClose = window;
                }

                if (windowToClose != null)
                    windowToClose.Close();
                else
                    _escapeKeyPressedTrigger.Fire();
            }
        }
    }

    public class WindowOpenedSignal : SmartWeakSignal<string>
    {
        public class Trigger : TriggerBase { }
    }

    public class WindowClosedSignal : SmartWeakSignal<string, WindowExitCode>
    {
        public class Trigger : TriggerBase { }
    }

    public class EscapeKeyPressedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
