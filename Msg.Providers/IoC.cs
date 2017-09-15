using Microsoft.Practices.Unity;

namespace Msg.Providers
{
    /// <summary>
    /// Locates and resolve dependent instances at runtime.
    /// </summary>
    /// 
    // [Serializable]
    public static class IoC
    {
     
        private static  IUnityContainer _container;

     
        /// <summary>
        /// Initializes the <see cref="IoC"/> class.
        /// </summary>
        static IoC()
        {
            ConfigureContainer();
        }


        public static IUnityContainer Current
        {
            get
            {
                return _container;
            }
        }

        private static void ConfigureContainer()
        {
            _container = new UnityContainer();

            _container.RegisterType<UnitOfWork.UnitOfWork>(new ContainerControlledLifetimeManager());
        }
    }
}

