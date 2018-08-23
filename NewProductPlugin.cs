using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.NewProduct.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.NewProduct
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class NewProductPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly NewProductSetting _settings;
        private readonly IMyProductService _productService;
        public NewProductPlugin(IPictureService pictureService, 
            ISettingService settingService, IWebHelper webHelper, NewProductSetting settings, IMyProductService productService)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            _settings = settings;
            _productService = productService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return string.IsNullOrEmpty(_settings.WidgetZone) ? new List<string>() : new List<string> { _settings.WidgetZone };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsNewProduct";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.NewProduct.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsNewProduct";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.NewProduct.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new NewProductSetting
            {
                SpecificationAttributeOptionId = _productService.GetSpecificationAttributeOptionId(),
                WidgetZone = "new_product"
            };
            _settingService.SaveSetting(settings);
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<NewProductSetting>();
            base.Uninstall();
        }
    }
}
