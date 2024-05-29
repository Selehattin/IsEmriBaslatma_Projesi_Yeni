using SAP.Middleware.Connector;

namespace IsEmriBaslatma_WebUI.Helpers
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
                parms.Add(RfcConfigParameters.SystemID, "ds4");
                parms.Add(RfcConfigParameters.User, "RFC_USER");
                parms.Add(RfcConfigParameters.Password, "Klmsn_2022!");
                parms.Add(RfcConfigParameters.Client, "110");
                parms.Add(RfcConfigParameters.Language, "TR");
                parms.Add(RfcConfigParameters.PoolSize, "5");
            }

            return parms;
        }
    }
}


//<!--<Server>172.20.22.192</Server>
//<SystemNumber>00</SystemNumber>
//<SystemID>Q4S</SystemID>
//<User>RFC_USER</User>
//<Password>Klmsn_2022!</Password>
//<Client>100</Client>
//<Name>SE37</Name>-->