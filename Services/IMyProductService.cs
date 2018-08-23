
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Widgets.NewProduct.Services
{
   public interface IMyProductService
    {
        int GetSpecificationAttributeOptionId();
       IList<Product> GetAllProductsDisplayedOnNewProductPage();
    }
}
