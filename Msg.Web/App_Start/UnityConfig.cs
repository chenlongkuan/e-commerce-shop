using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;

namespace Msg.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            //new PerRequestLifetimeManager()
            // TODO: Register your types here
            container.RegisterType<UsersHelper>();
            container.RegisterType<CommentsHelper>();
            container.RegisterType<BrandsHelper>(); 
            container.RegisterType<OrdersHelper>();
            container.RegisterType<NotifiesHelper>();
            container.RegisterType<CouponsHelper>();
            container.RegisterType<ProductsHelper>();
            container.RegisterType<ExchangesHelper>();
            container.RegisterType<CreditGoodsHelper>();
            container.RegisterType<GoodsHelper>();
            container.RegisterType<SchoolHelper>();
            container.RegisterType<CategoryHelper>();
            container.RegisterType<CommentsHelper>();
            container.RegisterType<CartHelper>();
            DisplayContainerRegistrations(container);
        }


        public static void DisplayContainerRegistrations(IUnityContainer theContainer)
        {
            string regName, regType, mapTo, lifetime;
            System.Diagnostics.Trace.TraceInformation(string.Format("容器中 {0} 个注册信息:",
                 theContainer.Registrations.Count()));
            foreach (ContainerRegistration item in theContainer.Registrations)
            {
                regType = item.RegisteredType.Name;
                mapTo = item.MappedToType.Name;
                regName = item.Name ?? "[默认]";
                lifetime = item.LifetimeManagerType.Name;
                if (mapTo != regType)
                {
                    mapTo = " -> " + mapTo;
                }
                else
                {
                    mapTo = string.Empty;
                }
                lifetime = lifetime.Substring(0, lifetime.Length - "生命周期管理器".Length);
                System.Diagnostics.Trace.TraceInformation(string.Format("+ {0}{1}  '{2}'  {3}", regType, mapTo, regName, lifetime));
            }
        }
    }
}
