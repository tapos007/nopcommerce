
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.NewProduct
{
    public class NewProductSetting : ISettings
    {
        public int SpecificationAttributeOptionId { get; set; }
        public string WidgetZone { get; set; }
        public bool UseNewProducts { get; set; }

    }
}
