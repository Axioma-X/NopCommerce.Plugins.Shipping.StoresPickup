using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.StoresPickup.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.StoresPickup.Fields.Stores")]
        public string Tiendas { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.StoresPickup.Fields.type")]
        public string Tipo { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.StoresPickup.Fields.get")]
        public bool RecogerTienda { get; set; }

    }
}
