using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.NewProduct.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [Required]
        [UIHint("NumberTemplate")]
        public int SpecificationAttributeOptionId { get; set; }

        [Required]
        public string WidgetZone { get; set; }

        public bool UseNewProducts { get; set; }
    }
}
