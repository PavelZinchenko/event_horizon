namespace GameDatabase.Model
{
    public interface IItemId
    {
        int Value { get; }
        bool IsNull { get; }
    }

    public struct ItemId<T> : IItemId
    {
        public ItemId(int id)
        {
            _id = id;
        }

        public int Value { get { return _id; } }
        public bool IsNull { get { return _id < 0; } }

        public override int GetHashCode() { return Value; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is ItemId<T>)
            {
                return _id == ((ItemId<T>)obj)._id;
            }

            if (obj is int)
            {
                return _id == (int)obj;
            }

            return false;
        }

        public static bool operator ==(ItemId<T> a, ItemId<T> b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(ItemId<T> a, ItemId<T> b)
        {
            return a.Value != b.Value;
        }

        public override string ToString() { return typeof(T).Name + "_" + (_id >= 0 ? _id.ToString() : "NULL"); }

        private readonly int _id;

        public static readonly ItemId<T> Empty = new ItemId<T>(-1);
    }
}
