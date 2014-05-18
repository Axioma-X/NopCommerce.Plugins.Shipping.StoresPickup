using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.StoresPickup.Class
{
    public class StoresPickup
    {
        public IEnumerable<String> Tiendas()
        {
            for(int i = 0; i <= 10; i++){
                string o = i.ToString();
                yield return o;
            }
        }

        public decimal ValorEnvio(string tienda)
        {
                decimal ValorEnvio = 0;
            var buscar = from a in Tiendas().ToList() where a == tienda select a;
            if(buscar.Count() > 0)
                ValorEnvio = 10000;
            return ValorEnvio;
        }
    }
}
