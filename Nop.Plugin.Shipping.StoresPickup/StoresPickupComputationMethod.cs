//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Shipping.StoresPickup
{
    /// <summary>
    /// UPS computation method
    /// </summary>
    public class StoresPickupComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Constants

        private const int MAXPACKAGEWEIGHT = 150;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "lb";
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";


        #endregion

        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly StoresPickupSettings _ffSettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public StoresPickupComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            StoresPickupSettings upsSettings, ICountryService countryService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IOrderTotalCalculationService orderTotalCalculationService, ILogger logger,
            ILocalizationService localizationService)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._ffSettings = upsSettings;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._logger = logger;
            this._localizationService = localizationService;
        }
        #endregion

        #region Methods
        private Localizer _localizer;
        public Localizer T
        {
            get
            {
                if (_localizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResource(format);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return
                            new LocalizedString((args == null || args.Length == 0)
                                                    ? resFormat
                                                    : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }
        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");
            var response = new GetShippingOptionResponse();
            var lista = _ffSettings.Tiendas.Split(';');
            foreach (var item in lista)
            {
                
                var option = new ShippingOption();
                option.Name = T("Plugins.Shipping.StoresPickup.Selection") + item.ToString();
                option.Rate = 0;
                response.ShippingOptions.Add(option);
            }
           
            // Lista de tiendas 

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError(T("Plugins.Shipping.StoresPickup.NoItems")+"");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError(T("Plugins.Shipping.StoresPickup.NoAddress")+"");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError(T("Plugins.Shipping.StoresPickup.NoCountry") + "");
                return response;
            }

            return response;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {            return null;
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
            controllerName = "ShippingStoresPickup";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.StoresPickup.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new StoresPickupSettings()
            {
                Tiendas = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.Fields.Stores", "List of stores");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.Fields.Stores.Hint", "Your store list separated by semicolon (e.g. store 1; store 2;…)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.Selection", "Pick up on Store: ");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.NoItems", "No shipment items");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.NoAddress", "Shipping address is not set");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.StoresPickup.NoCountry", "Shipping country is not set");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<StoresPickupSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.Fields.Stores");
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.Fields.Stores.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.Selection");
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.NoItems");
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.NoAddress");
            this.DeletePluginLocaleResource("Plugins.Shipping.StoresPickup.NoCountry");
            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Offline;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}