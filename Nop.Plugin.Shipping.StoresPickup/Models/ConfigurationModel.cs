using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.StoresPickup.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.StoresPickup.Fields.Tiendas")]
        public string Tiendas { get; set; }
        public string Tipo { get; set; }

        public bool RecogerTienda { get; set; }
    }
}
