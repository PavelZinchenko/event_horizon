namespace Zenject
{
    public interface IBindingFinalizer
    {
        bool InheritInSubContainers
        {
            get;
        }

        void FinalizeBinding(DiContainer container);
    }
}
