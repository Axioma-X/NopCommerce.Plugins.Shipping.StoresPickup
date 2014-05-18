using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.StoresPickup.Models
{
    public class StoresPickupInfoModel : BaseNopModel
    {
        public StoresPickupInfoModel()
        {
            ListaTiendas = new List<SelectListItem>();
           
        }

        public IList<SelectListItem> ListaTiendas { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.StoresPickup.Fields.Tiendas")]
        public string Tiendas { get; set; }
        public string Tipo { get; set; }

        public bool RecogerTienda { get; set; }
    }
}
