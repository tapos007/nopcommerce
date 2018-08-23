using System.Data.Entity;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.NewProduct.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            Database.SetInitializer<PluginObjectContext>(null);
        }

        public int Order
        {
            get { return 0; }
        }
    }
}