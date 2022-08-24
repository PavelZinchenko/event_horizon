namespace Services.Gui
{
    public enum EscapeKeyAction
    {
        None,
        Block,
        Close,
    }

    public interface IWindow
    {
        string Id { get; }
        WindowClass Class { get; }

		void Open();
		void Open(WindowArgs args);
        void Close();
        
        bool IsVisible { get; }
        bool Enabled { get; set; }

        EscapeKeyAction EscapeAction { get; }
    }
}
