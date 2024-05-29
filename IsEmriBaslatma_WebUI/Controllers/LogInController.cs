using IsEmriBaslatma_BusinessLayer.EfManagers;
using IsEmriBaslatma_DataAccesLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IsEmriBaslatma_WebUI.Controllers
{

    public class LogInController : Controller
    {
        private readonly EFLoginManager _EFLoginManager = new EFLoginManager();
        //GET: LogIn
        public ActionResult LogIn()
        {
            ViewBag.LogInError = TempData["hata"];
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(string userName, string password, string vardiya)
        {
            try
            {
                List<KULLANICILAR> login = _EFLoginManager.getAll();
                if (login.Any(x => x.SICIL_NO == userName && x.SIFRE == password))
                {
                    if (login.Any(x => x.SICIL_NO == userName && x.SIFRE == password && x.KULLANICI_LOG==true))
                    {
                        TempData["hata"] = "Oturum suanda açık.";
                        return RedirectToAction("LogIn");   
                    }
                   
                    
                    KULLANICILAR user = login.FirstOrDefault(x => x.SICIL_NO == userName && x.SIFRE == password);
                    //user.KULLANICI_LOG=true;
                    _EFLoginManager.update(user);
                    Session["user"] = user;
                    if (user.ROL_ID == 1) // admin toll
                    {
                    }
                    else if (user.ROL_ID == 2)
                    {
                        return Redirect("~/Home/ArgeSapmaListesi/");
                    }
                    else if (user.ROL_ID == 3)
                    {
                        return Redirect("~/Home/IsEmriBaslatma/");
                    }
                    return RedirectToAction("IsEmriBaslatma", "Home");
                }
                else
                {
                    TempData["hata"] = "Kullanici veya adı yanlış girildi.";
                    return RedirectToAction("LogIn");
                }
            }
            catch (System.Exception)
            {

                throw;
            }
            return View();
        }
        public ActionResult LogOut()
        {
            KULLANICILAR user=(KULLANICILAR) Session["user"];
            user.KULLANICI_LOG = false;
            _EFLoginManager.update(user);
            Session["user"] = null;
            return Session["user"] == null ? Redirect("LogIn") : (ActionResult)View();
        }

        // Sicil numarasına göre kullanıcının rolünü kontrol eden endpoint
        [HttpPost]
        public ActionResult CheckUserRole(string sicilNo)
        {
            try
            {
                // Sicil numarasına göre kullanıcıyı bul
                KULLANICILAR user = _EFLoginManager.getAll().FirstOrDefault(x => x.SICIL_NO == sicilNo);
                int? rolId = null;
                if (user != null)
                {
                    rolId = user.ROL_ID;
                }

                // JSON formatında yanıt döndür
                return Json(new { rolId });
            }
            catch (Exception)
            {
                // Hata oluştuğunda boş bir JSON döndür
                return Json(new { rolId = "" });
            }
        }

    }
}