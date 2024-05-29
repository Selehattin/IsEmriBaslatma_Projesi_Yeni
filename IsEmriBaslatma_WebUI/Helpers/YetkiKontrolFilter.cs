using IsEmriBaslatma_DataAccesLayer.Model;
using System.Web;
using System.Web.Mvc;

namespace IsEmriBaslatma_WebUI.Helpers
{
    public class YetkiKontrolFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // HTTP isteğinden session'a erişim
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            KULLANICILAR kullanici = (KULLANICILAR)session["user"];
            // Session'da kullanıcı nesnesi ve rol ID'si var mı kontrol ediyoruz


            // Kullanıcının rol ID'si 2 ise, belirli bir sayfaya yönlendir
            if (kullanici.ROL_ID == 2 && kullanici.ROL_ID == 1)
            {
                filterContext.Result = new RedirectResult("~/Home/ArgeSapmaListesi/"); // Örnek olarak BelirliSayfa'ya yönlendiriyoruz
                return;
            }
            if (kullanici.ROL_ID == 3)
            {
                filterContext.Result = new RedirectResult("~/Home/IsEmriBaslatma/"); // Örnek olarak BelirliSayfa'ya yönlendiriyoruz
                return;
            }
            base.OnActionExecuting(filterContext);
        }


    }
}