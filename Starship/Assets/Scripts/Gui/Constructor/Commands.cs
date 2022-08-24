using System.Collections.Generic;
using System.Linq;
using Constructor;
using UnityEngine;
using ViewModel;

namespace Gui.Constructor
{
    public interface ICommand
    {
        bool TryExecute();
        bool TryRollback();
    }

    public class InstallComponentCommand : ICommand
    {
        public InstallComponentCommand(ShipLayoutViewModel layout, Vector2 position, ComponentInfo component, int keyBinding, int behaviour)
        {
            _layout = layout;
            _position = position;
            _component = component;
            _keyBinding = keyBinding;
            _behaviour = behaviour;
        }

        public static bool TryExecuteCommand(ShipLayoutViewModel layout, Vector2 position, ComponentInfo component, int keyBinding, int behaviour, ConstructorViewModel.CommandEvent onCommandEvent)
        {
            var command = new InstallComponentCommand(layout, position, component, keyBinding, behaviour);
            if (command.TryExecute())
            {
                onCommandEvent.Invoke(command);
                return true;
            }

            return false;
        }

        public bool TryExecute()
        {
            _id = _layout.InstallComponent(_position, _component, _keyBinding, _behaviour);
            return _id >= 0;
        }

        public bool TryRollback()
        {
            _layout.RemoveComponent(_id);
            return true;
        }

        private int _id = -1;
        private readonly Vector2 _position;
        private readonly int _keyBinding;
        private readonly int _behaviour;
        private readonly ComponentInfo _component;
        private readonly ShipLayoutViewModel _layout;
    }

    public class RemoveComponentCommand : ICommand
    {
        public RemoveComponentCommand(ShipLayoutViewModel layout, int id)
        {
            _layout = layout;
            _id = id;
        }

        public bool TryExecute()
        {
            var component = _layout.Layout.GetComponent(_id);
            if (component == null)
                return false;

            _x = component.X;
            _y = component.Y;
            _behaviour = component.Behaviour;
            _keyBinding = component.KeyBinding;
            _component = component.Info;

            _layout.RemoveComponent(_id);

            return true;
        }

        public bool TryRollback()
        {
            return _id == _layout.InstallComponent(_x, _y, _component, _keyBinding, _behaviour, _id);
        }

        private int _x;
        private int _y;
        private int _keyBinding;
        private int _behaviour;
        private readonly int _id;
        private ComponentInfo _component;
        private readonly ShipLayoutViewModel _layout;
    }

    public class ComplexCommand : ICommand
    {
        public ComplexCommand(params ICommand[] commands)
        {
            _commands = new List<ICommand>(commands.Where(item => item != null));
        }

        public bool TryExecute()
        {
            for (var i = 0; i < _commands.Count; ++i)
                if (!_commands[i].TryExecute())
                    return false;

            return true;
        }

        public bool TryRollback()
        {
            for (var i = _commands.Count - 1; i >= 0; --i)
                if (!_commands[i].TryRollback())
                    return false;

            return true;
        }

        private readonly List<ICommand> _commands;
    }
}
