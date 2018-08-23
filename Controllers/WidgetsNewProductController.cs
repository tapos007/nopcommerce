using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.Widgets.NewProduct.Models;
using Nop.Plugin.Widgets.NewProduct.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Portable.Licensing;
using Portable.Licensing.Validation;

namespace Nop.Plugin.Widgets.NewProduct.Controllers
{
    public class WidgetsNewProductController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;
        
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly NewProductSetting _setting;
        private readonly IMyProductService _service;

        public WidgetsNewProductController(IWorkContext workContext,
            IStoreContext storeContext,
            IPictureService pictureService, ISettingService settingService, CatalogSettings catalogSettings,
            IProductService productService, ILocalizationService localizationService, MediaSettings mediaSettings,
            IWebHelper webHelper, IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository, NewProductSetting setting, IMyProductService service)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pictureService = pictureService;
            this._settingService = settingService;
            _catalogSettings = catalogSettings;
            _productService = productService;
            _localizationService = localizationService;
            _mediaSettings = mediaSettings;
            _webHelper = webHelper;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _service = service;
            _setting = setting;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                SpecificationAttributeOptionId = _setting.SpecificationAttributeOptionId,
                WidgetZone = _setting.WidgetZone,
                UseNewProducts = _setting.UseNewProducts
            };
            return View("~/Plugins/Widgets.NewProduct/Views/WidgetsNewProduct/Configure.cshtml", model);
        }
        [AdminAuthorize]
        [ChildActionOnly]
        [HttpPost]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (ModelState.IsValid)
            {
                _setting.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;
                _setting.WidgetZone = model.WidgetZone;
                _setting.UseNewProducts = model.UseNewProducts;
                _settingService.SaveSetting(_setting);
            }
            return Configure();
        }

        protected IEnumerable<ProductOverviewModel> PrepareNewProducsModels(IEnumerable<Product> products,
           bool preparePictureModel = true, int? productThumbPictureSize = null)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            //performance optimization. let's load all variants at one go
            //var allVariants = _productService.GetProductVariantsByProductIds(products.Select(x => x.Id).ToArray());


            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel()
                {
                    Id = product.Id,
                    Name = product.GetLocalized(x => x.Name),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
                    FullDescription = product.GetLocalized(x => x.FullDescription),
                    SeName = product.GetSeName(),
                };


                //picture
                if (preparePictureModel)
                {
                    #region Prepare product picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : _mediaSettings.ProductThumbPictureSize;
                    //prepare picture model

                    var picture = _pictureService.GetPicturesByProductId(product.Id);
                    model.DefaultPictureModel = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture[0], pictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture[0]),
                            Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name)
                        };


                    #endregion
                }

                models.Add(model);
            }
            return models;
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            if (System.Web.HttpContext.Current.Session["DevPartnerLicense"] == null)
            {
                var licenseText = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/dev-partner.biz.lic"));
                System.Web.HttpContext.Current.Session["DevPartnerLicense"] = License.Load(licenseText);
            }
            var license = System.Web.HttpContext.Current.Session["DevPartnerLicense"] as License;
            var validationFailures = license.Validate()
                                    .ExpirationDate()
                                        .When(lic => lic.Type == LicenseType.Trial)
                                        .And()
                                    .AssertThat(lic => lic.ProductFeatures.Get("Hosts").Contains(System.Web.HttpContext.Current.Request.Url.Host),
                                                new GeneralValidationFailure { Message = "Host does not match!" })
                                    .And()
                                    .AssertThat(lic => lic.ProductFeatures.Get(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) == "Yes",
                                                new GeneralValidationFailure { Message = "You don't have license to use this product!" })
                                    .And()
                                    .Signature("MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABCDWWmzm7F6PHY+daPxsBoJVgeysDdR0lXJuEdKMvB9HhwuGZURbFKVdaeo2bODDj2MiAoMDIxCDPt/OPMEueBM=")
                                    .AssertValidLicense();
            if (validationFailures.Any())
            {
                var stringBuilder = new StringBuilder();
                foreach (var failure in validationFailures)
                    stringBuilder.AppendLine(failure.GetType().Name + ": " + failure.Message + " - " + failure.HowToResolve);
                throw new NopException(stringBuilder.ToString());
            }
            var model = new List<ProductOverviewModel>();
            //var serv = new MyProductService(_productSpecificationAttributeRepository,);

            if (_catalogSettings.RecentlyAddedProductsEnabled)
            {
                var products = _service.GetAllProductsDisplayedOnNewProductPage();
                model.AddRange(PrepareNewProducsModels(products));
            }

            return View("~/Plugins/Widgets.NewProduct/Views/WidgetsNewProduct/PublicInfo.cshtml", model);
        }
    }
}