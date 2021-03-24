using SimpleInjector;
using System;
using System.Collections.Generic;

namespace Basket.IoC
{
    public class ContainerHandler
    {
        #region Public Constructors

        static ContainerHandler()
        {
            Instance = new ContainerHandler();
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected ContainerHandler()
        {
            Container = new Container();
        }

        #endregion Protected Constructors

        #region Public Events

        public event EventHandler<ContainerChangedEventArgs> ContainerReloaded;

        #endregion Public Events

        #region Public Properties

        public static ContainerHandler Instance { get; }

        public Container Container { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void SetupConfigurationDependencies(IEnumerable<DIDefinition> definitions)
        {
            foreach (var definition in definitions) SetupDependency(definition);
        }

        public void SetupDependency(DIDefinition definition)
        {
            var serviceType = definition.ServiceTypeObj;
            if (serviceType != null)
            {
                var implType = definition.ImplementationTypeObj;
                if (implType != null)
                {
                    if (definition.Args == null)
                    {
                        Container.Register(serviceType, implType, GetLifestyle(definition.DIScope));
                    }
                    else
                    {
                        var simpleGeneratorFactory = new GeneratorGeneratorFactory<object>(() => Activator.CreateInstance(implType, definition.Args));
                        Container.Register(serviceType, () => simpleGeneratorFactory.Create(), GetLifestyle(definition.DIScope));
                    }
                }
            }
        }

        #endregion Public Methods

        #region İnternal Methods

        internal Container GenerateNewContainer()
        {
            return new Container();
        }

        internal void ReplaceContainer(Container container)
        {
            ContainerReloaded?.Invoke(this, new ContainerChangedEventArgs
            {
                Container = container
            });
            Container = container;
        }

        #endregion İnternal Methods

        #region Private Methods

        private Lifestyle GetLifestyle(DIDefinition.Scope scope)
        {
            switch (scope)
            {
                case DIDefinition.Scope.Singleton:
                    return Lifestyle.Singleton;

                case DIDefinition.Scope.Scoped:
                    return Lifestyle.Scoped;

                default:
                    return Lifestyle.Transient;
            }
        }

        #endregion Private Methods
    }
}