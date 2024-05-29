using System.Web.Mvc;

namespace IsEmriBaslatma_WebUI.Helpers
{
    public class AuthFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["user"] == null)
            {
                filterContext.Result = new RedirectResult("/LogIn/LogIn");
            }
        }
    }
}