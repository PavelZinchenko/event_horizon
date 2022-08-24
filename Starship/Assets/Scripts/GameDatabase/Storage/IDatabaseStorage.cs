namespace GameDatabase.Storage
{
    public partial interface IDataStorage
    {
        string Name { get; }
        string Id { get; }
        int SchemaVersion { get; }
        bool IsEditable { get; }

        void UpdateItem(string name, string content);
    }
}
