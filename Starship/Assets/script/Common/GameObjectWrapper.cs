namespace Common
{
    public struct GameObjectWrapper<T> 
        where T : UnityEngine.Object
    {
        public GameObjectWrapper(T gameObject)
        {
            _gameObject = gameObject;
            _notNull = gameObject != null;
        }

        public static implicit operator bool(GameObjectWrapper<T> wrapper)
        {
            return wrapper._notNull;
        }

        //public static implicit operator GameObjectWrapper<T>(T value)
        //{
        //    return new GameObjectWrapper<T>(value);
        //}

        public static implicit operator T(GameObjectWrapper<T> wrapper)
        {
            return wrapper._notNull ? wrapper._gameObject : null;
        }

        private readonly bool _notNull;
        private readonly T _gameObject;
    }
}
