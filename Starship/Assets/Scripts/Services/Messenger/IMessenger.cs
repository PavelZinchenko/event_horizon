namespace Services.Messenger
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, U>(T arg1, U arg2);
    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    public interface IMessenger
    {
        void AddListener(EventType eventType, Callback handler);
        void AddListener<T>(EventType eventType, Callback<T> handler);
        void AddListener<T, U>(EventType eventType, Callback<T, U> handler);
        void AddListener<T, U, V>(EventType eventType, Callback<T, U, V> handler);

        void RemoveListener(EventType eventType, Callback handler);
        void RemoveListener<T>(EventType eventType, Callback<T> handler);
        void RemoveListener<T, U>(EventType eventType, Callback<T, U> handler);
        void RemoveListener<T, U, V>(EventType eventType, Callback<T, U, V> handler);

        void Broadcast(EventType eventType);
        void Broadcast<T>(EventType eventType, T arg1);
        void Broadcast<T, U>(EventType eventType, T arg1, U arg2);
        void Broadcast<T, U, V>(EventType eventType, T arg1, U arg2, V arg3);
    }
}
