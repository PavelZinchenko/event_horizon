using System;

namespace Zenject
{
    // Four parameters

    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
    {
        public CommandBinder(string identifier, DiContainer container)
            : base(identifier, container)
        {
        }

        public ScopeArgBinder To<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                BindInfo, methodGetter,
                (container) => new TransientProvider(
                    typeof(THandler), container, BindInfo.Arguments, BindInfo.ConcreteIdentifier));

            return new ScopeArgBinder(BindInfo);
        }

        public ScopeBinder ToResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, false);
        }

        public ScopeBinder ToOptionalResolve<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToOptionalResolve<THandler>(null, methodGetter);
        }

        public ScopeBinder ToOptionalResolve<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToResolveInternal<THandler>(identifier, methodGetter, true);
        }

        public ConditionBinder ToNothing()
        {
            return ToMethod((p1, p2, p3, p4) => {});
        }

        // AsSingle / AsCached / etc. don't make sense in this case so just return ConditionBinder
        public ConditionBinder ToMethod(Action<TParam1, TParam2, TParam3, TParam4> action)
        {
            // Create the command class once and re-use it everywhere
            Finalizer = new SingleProviderBindingFinalizer(
                BindInfo,
                (container, _) => new CachedProvider(
                    new TransientProvider(
                        typeof(TCommand), container,
                        InjectUtil.CreateArgListExplicit(action), null)));

            return new ConditionBinder(BindInfo);
        }

        ScopeBinder ToResolveInternal<THandler>(
            string identifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter, bool optional)
        {
            Finalizer = new CommandBindingFinalizer<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                BindInfo, methodGetter,
                (container) => new ResolveProvider(typeof(THandler), container, identifier, optional));

            return new ScopeBinder(BindInfo);
        }
    }
}




