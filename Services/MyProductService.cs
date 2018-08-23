
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.NewProduct.Data;


namespace Nop.Plugin.Widgets.NewProduct.Services
{
    public partial class MyProductService : IMyProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly PluginObjectContext _pluginObjectContext;
        private readonly NewProductSetting _setting;

        public MyProductService( IRepository<Product> productRepository, IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository, PluginObjectContext pluginObjectContext, NewProductSetting settings)
        {
            _productRepository = productRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _pluginObjectContext = pluginObjectContext;
            _setting = settings;
        }


        public int GetSpecificationAttributeOptionId()
        {

            var dbScript = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Plugins/Widgets.NewProduct/sql/script.sql"));
            var specifAttrOptionId = _pluginObjectContext.Database.SqlQuery<int>(dbScript).ToList()[0];
            return specifAttrOptionId;
        }

        /// <summary>
        /// Gets all products displayed on the  NewProduct page
        /// </summary>
        /// <returns>Product collection</returns>
        /// 
        public virtual IList<Product> GetAllProductsDisplayedOnNewProductPage()
        {
            if (_setting.UseNewProducts)
            {
                    var qqq = (from p in _productRepository.Table
                              where !p.Deleted
                              orderby p.CreatedOnUtc descending
                              select p).Take(9);
                    var products = qqq.ToList();
                    return products;
            }
            else
            {
                if (_setting.SpecificationAttributeOptionId > 0)
                {
                    var qqq = (from p in _productSpecificationAttributeRepository.Table
                        where
                            p.SpecificationAttributeOptionId == _setting.SpecificationAttributeOptionId &&
                            !p.Product.Deleted
                        orderby p.Product.UpdatedOnUtc descending
                               select p.Product).Take(9);
                    var products = qqq.ToList();
                    return products;
                }
            }
            return new List<Product>();
        }
    }
}
