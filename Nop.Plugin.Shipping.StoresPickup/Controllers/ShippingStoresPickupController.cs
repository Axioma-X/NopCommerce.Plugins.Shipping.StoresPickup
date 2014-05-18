using System;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.StoresPickup.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Shipping.StoresPickup.Controllers
{
    [AdminAuthorize]
    public class ShippingStoresPickupController : Controller
    {
        private readonly StoresPickupSettings _ffSettings;
        private readonly ISettingService _settingService;
       // private readonly ICountryService _countryService;

        public ShippingStoresPickupController(StoresPickupSettings ffSettings, ISettingService settingService,
            ICountryService countryService)
        {
            this._ffSettings =  ffSettings;
            this._settingService = settingService;
           // this._countryService = countryService;
         //   Class.FF ff = new Class.FF();
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            ConfigurationModel model = new ConfigurationModel();
            model.Tiendas = _ffSettings.Tiendas;
         
            ViewBag.Debug = "Prueba del controlador";
            return View("Nop.Plugin.Shipping.StoresPickup.Views.ShippingStoresPickup.Configure", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _ffSettings.Tiendas = model.Tiendas;

            _settingService.SaveSetting(_ffSettings);

            return Configure();
        }


          //[ChildActionOnly]
          // public ActionResult shipingmethod()
          // {
               //var model = new Sto FFInfoModel();
              /*
               //CC types
               model.CreditCardTypes.Add(new SelectListItem()
               {
                   Text = "Visa",
                   Value = "Visa",
               });
               model.CreditCardTypes.Add(new SelectListItem()
               {
                   Text = "Master card",
                   Value = "MasterCard",
               });
               model.CreditCardTypes.Add(new SelectListItem()
               {
                   Text = "Discover",
                   Value = "Discover",
               });
               model.CreditCardTypes.Add(new SelectListItem()
               {
                   Text = "Amex",
                   Value = "Amex",
               });

               //years
               for (int i = 0; i < 15; i++)
               {
                   string year = Convert.ToString(DateTime.Now.Year + i);
                   model.ExpireYears.Add(new SelectListItem()
                   {
                       Text = year,
                       Value = year,
                   });
               }

               //months
               for (int i = 1; i <= 12; i++)
               {
                   string text = (i < 10) ? "0" + i.ToString() : i.ToString();
                   model.ExpireMonths.Add(new SelectListItem()
                   {
                       Text = text,
                       Value = i.ToString(),
                   });
               }

               //set postback values
               var form = this.Request.Form;
               model.CardholderName = form["CardholderName"];
               model.CardNumber = form["CardNumber"];
               model.CardCode = form["CardCode"];
               var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
               if (selectedCcType != null)
                   selectedCcType.Selected = true;
               var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
               if (selectedMonth != null)
                   selectedMonth.Selected = true;
               var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
               if (selectedYear != null)
                   selectedYear.Selected = true;*/

          //     return View("Nop.Plugin.Shipping.FF.Views.ShippingFF.FFInfo");
          // }

    }
}
