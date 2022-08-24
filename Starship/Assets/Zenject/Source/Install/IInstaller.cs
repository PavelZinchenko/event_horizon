namespace Zenject
{
    // We extract the interface so that monobehaviours can be installers
    public interface IInstaller
    {
        void InstallBindings();

        bool IsEnabled
        {
            get;
        }
    }

    // These interfaces are used for generic constraints to allow us to have strongly
    // typed parameters
    public interface IInstallerZeroParams : IInstaller
    {
    }

    public interface IInstallerOneParams : IInstaller
    {
    }

    public interface IInstallerTwoParams : IInstaller
    {
    }

    public interface IInstallerThreeParams : IInstaller
    {
    }

    public interface IInstallerFourParams : IInstaller
    {
    }

    public interface IInstallerFiveParams : IInstaller
    {
    }

}
