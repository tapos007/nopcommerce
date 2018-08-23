using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.NewProduct.Data;
using Nop.Plugin.Widgets.NewProduct.Services;

namespace Nop.Plugin.Widgets.NewProduct
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string ContextName = "nop_object_context_new_product";

        public void Register(ContainerBuilder builder, Core.Infrastructure.ITypeFinder typeFinder)
        {
         //   builder.RegisterType<MyProductService>().As<IMyProductService>().InstancePerHttpRequest();
        
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                //register named context
                builder.Register<IDbContext>(c => new PluginObjectContext(dataProviderSettings.DataConnectionString))
                    .Named<IDbContext>(ContextName)
                    .InstancePerHttpRequest();

                builder.Register(
                    c => new PluginObjectContext(dataProviderSettings.DataConnectionString))
                    .InstancePerHttpRequest();

              
            }
            else
            {
                //register named context
                builder.Register<IDbContext>(
                    c => new PluginObjectContext(c.Resolve<DataSettings>().DataConnectionString))
                    .Named<IDbContext>(ContextName)
                    .InstancePerHttpRequest();
                builder.Register(
                    c => new PluginObjectContext(c.Resolve<DataSettings>().DataConnectionString))
                    .InstancePerHttpRequest();
            }
            builder.RegisterType<MyProductService>().As<IMyProductService>().InstancePerHttpRequest(); 
          
        }

        public int Order
        {
            get { return 1; }
        }
    }
}