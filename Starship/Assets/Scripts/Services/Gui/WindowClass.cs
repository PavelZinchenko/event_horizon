namespace Services.Gui
{
    public enum WindowClass
    {
        HudElement,
        TopLevel,
        Singleton,
        Level2,
        ModalDialog,
        Balloon,
    }

    public static class WindowClassExtensions
    {
        public static bool MustBeDisabledDueTo(this WindowClass myClass, WindowClass otherClass)
        {
            if (myClass == WindowClass.ModalDialog || myClass == WindowClass.Balloon)
                return false;
            if (otherClass == WindowClass.ModalDialog || otherClass == WindowClass.TopLevel)
                return true;

            return false;
        }

        public static bool MustBeClosedDueTo(this WindowClass myClass, WindowClass otherClass)
        {
            if (myClass == WindowClass.ModalDialog)
                return false;
            if (myClass == WindowClass.Balloon)
                return otherClass == WindowClass.Balloon;
            if (otherClass == WindowClass.TopLevel)
                return true;
            if (otherClass == WindowClass.Singleton)
                return myClass == WindowClass.Singleton || myClass == WindowClass.Level2;

            return false;
        }

        public static bool CantBeOpenedDueTo(this WindowClass myClass, WindowClass otherClass)
        {
            if (myClass == WindowClass.Balloon)
                return false;
            if (otherClass == WindowClass.ModalDialog)
                return true;
            if (myClass == WindowClass.ModalDialog)
                return false;
            if (otherClass == WindowClass.TopLevel)
                return myClass != WindowClass.TopLevel;

            return false;
        }

        public static bool MustBeOpenedAutomatically(this WindowClass windowClass)
        {
            return windowClass == WindowClass.Singleton || windowClass == WindowClass.HudElement;
        }
    }
}
