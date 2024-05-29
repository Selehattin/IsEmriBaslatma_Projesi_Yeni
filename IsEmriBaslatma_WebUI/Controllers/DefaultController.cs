//using SAP.Middleware.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.UI.WebControls;

//namespace IsEmriBaslatma_WebUI.Controllers
//{
//    public class DefaultController : Controller
//    {
//        // GET: Default
//        public ActionResult Index()
//        {
//            ECCDestinationConfig cfg = null;
//            RfcDestination dest = null;

//            try
//            {
//                cfg = new ECCDestinationConfig();
//                RfcDestinationManager.RegisterDestinationConfiguration(cfg);
//                dest = RfcDestinationManager.GetDestination("rfc");

//                RfcRepository repo = dest.Repository;

//                IRfcFunction fnpush = repo.CreateFunction("ZPP_SIPARIS_NO");
//                fnpush.SetValue("IV_SERI_NO", "A00066230312300025");

//                fnpush.Invoke(dest);

//                String res = fnpush.GetValue("EV_SIPARIS_NO").ToString();

//                RfcSessionManager.EndContext(dest);
//                RfcDestinationManager.UnregisterDestinationConfiguration(cfg);
//            }
//            catch (Exception ex)
//            {

//            }
//            return View();
//        }
//    } 

//    public class ECCDestinationConfig : IDestinationConfiguration
//        {
//            public bool ChangeEventsSupported()
//            {
//                return true;
//            }

//            public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;


//            public RfcConfigParameters GetParameters(string destionationName)
//            {
//                RfcConfigParameters parms = new RfcConfigParameters();

//                if (destionationName.Equals("rfc"))
//                {
//                    parms.Add(RfcConfigParameters.AppServerHost, "172.20.22.246");
//                    parms.Add(RfcConfigParameters.SystemNumber, "00");
//                    parms.Add(RfcConfigParameters.SystemID, "DS4");
//                    parms.Add(RfcConfigParameters.User, "RFC_USER");
//                    parms.Add(RfcConfigParameters.Password, "Klmsn_2022!");
//                    parms.Add(RfcConfigParameters.Client, "110");
//                    parms.Add(RfcConfigParameters.Language, "EN");
//                    parms.Add(RfcConfigParameters.PoolSize, "5");
//                }

//                return parms;
//            }
//        }
//}


