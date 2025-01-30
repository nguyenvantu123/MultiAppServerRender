using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAppServer.ServiceDefaults
{
    public class AppConfiguration
    {
        public string Secret { get; set; }

        public bool BehindSSLProxy { get; set; }

        public string ProxyIP { get; set; }

        public string ApplicationUrl { get; set; }
    }
}
