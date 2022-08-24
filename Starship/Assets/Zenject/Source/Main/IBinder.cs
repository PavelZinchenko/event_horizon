using System;
using System.Collections.Generic;

namespace Zenject
{
    public interface IBinder
    {
        // _____ Bind<> _____
        // Map the given type to a way of obtaining it
        // See ITypeBinderOld.cs for the full list of methods to call on the return value
        // Note that this can include open generic types as well such as List<>
        ConcreteIdBinderGeneric<TContract> Bind<TContract>();

        // _____ Bind _____
        // Non-generic version of Bind<> for cases where you only have the runtime type
        // Note that this can include open generic types as well such as List<>
        ConcreteIdBinderNonGeneric Bind(params Type[] contractTypes);

        // _____ BindFactory<> _____
        // TBD
        FactoryToChoiceIdBinder<TContract> BindFactory<TContract, TFactory>()
            where TFactory : Factory<TContract>;

        FactoryToChoiceIdBinder<TParam1, TContract> BindFactory<TParam1, TContract, TFactory>()
            where TFactory : Factory<TParam1, TContract>;

        FactoryToChoiceIdBinder<TParam1, TParam2, TContract> BindFactory<TParam1, TParam2, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TContract>;

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TContract> BindFactory<TParam1, TParam2, TParam3, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TContract>;

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TContract>;

        FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>;

        // _____ BindAllInterfaces _____
        // Bind all the interfaces for the given type to the same thing.
        //
        // Example:
        //
        //    public class Foo : ITickable, IInitializable
        //    {
        //    }
        //
        //    Container.BindAllInterfaces<Foo>().To<Foo>().AsSingle();
        //
        //  This line above is equivalent to the following:
        //
        //    Container.Bind<ITickable>().ToSingle<Foo>();
        //    Container.Bind<IInitializable>().ToSingle<Foo>();
        //
        // Note here that we do not bind Foo to itself.  For that, use BindAllInterfacesAndSelf
        ConcreteIdBinderNonGeneric BindAllInterfaces<T>();
        ConcreteIdBinderNonGeneric BindAllInterfaces(Type type);

        // _____ BindAllInterfacesAndSelf _____
        // Same as BindAllInterfaces except also binds to self
        ConcreteIdBinderNonGeneric BindAllInterfacesAndSelf<T>();
        ConcreteIdBinderNonGeneric BindAllInterfacesAndSelf(Type type);

        // _____ BindInstance _____
        //  This is simply a shortcut to using the FromInstance method.
        //
        //  Example:
        //      Container.BindInstance(new Foo());
        //
        //  This line above is equivalent to the following:
        //
        //      Container.Bind<Foo>().FromInstance(new Foo());
        //
        IdScopeBinder BindInstance<TContract>(TContract obj);

        // _____ HasBinding _____
        // Returns true if the given type is bound to something in the container
        bool HasBinding(InjectContext context);
        bool HasBinding<TContract>();
        bool HasBinding<TContract>(object identifier);
    }
}
