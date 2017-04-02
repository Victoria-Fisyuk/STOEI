using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using TAiMStore.Model.Factory;
using TAiMStore.Model.Repository;
using TAiMStore.Model.UnitOfWork;
using System.Web.ModelBinding;

namespace TAiMStore.Configs
{
    public class AutofacConfiguration
    {
        public static AutofacDependencyResolver GetAutofacDependencyResolver()
        {
            const string repository = "Repository";
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterType<Factory>().As<IFactory>().InstancePerHttpRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerHttpRequest();
            builder.RegisterType<EmailOrderProcessor>().As<IOrderProcessor>().InstancePerHttpRequest();

            builder.RegisterAssemblyTypes(typeof(ProductRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(RoleRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(ContactsRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(CategoryRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(OrderRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(OrderProductRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(PaymentRepository).Assembly)
                  .Where(t => t.Name.EndsWith(repository))
                  .AsImplementedInterfaces()
                  .InstancePerHttpRequest();
                        
            return new AutofacDependencyResolver(builder.Build());
        }
    }
}