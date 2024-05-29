using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsEmriBaslatma_WebUI.Models
{
    public class ECCDestinationConfig : IDestinationConfiguration
    {
        public bool ChangeEventsSupported()
        {
            return true;
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;


        public RfcConfigParameters GetParameters(string destionationName)
        {
            RfcConfigParameters parms = new RfcConfigParameters();

            if (destionationName.Equals("rfc"))
            {
                parms.Add(RfcConfigParameters.AppServerHost, "172.20.22.246");
                parms.Add(RfcConfigParameters.SystemNumber, "00");
                parms.Add(RfcConfigParameters.SystemID, "DS4");
                parms.Add(RfcConfigParameters.User, "RFC_USER");
                parms.Add(RfcConfigParameters.Password, "Klmsn_2022!");
                parms.Add(RfcConfigParameters.Client, "110");
                parms.Add(RfcConfigParameters.Language, "EN");
                parms.Add(RfcConfigParameters.PoolSize, "5");
            }

            return parms;
        }
    }
}




