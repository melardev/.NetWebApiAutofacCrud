using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using WebApiAutofacCrud.Data;
using WebApiAutofacCrud.Infrastructure.Services;

namespace WebApiAutofacCrud
{
    public static class AutofacWebApiConfig
    {
        private static IContainer _container;
        public static void Configure()
        {
            _container = RegisterServices(new ContainerBuilder());
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_container);  
        }
        
        private static IContainer RegisterServices(ContainerBuilder builder)  
        {  
            // Register Web Api Controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ApplicationDbContext>()
                .As<ApplicationDbContext>()
                .InstancePerRequest();

            builder.RegisterType<TodoService>()
                .As<ITodoService>()
                .InstancePerRequest();


            return builder.Build();
        }
        
    }
}