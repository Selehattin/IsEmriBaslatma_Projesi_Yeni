using IsEmriBaslatma_BusinessLayer.EfManagers;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_WebUI.Helpers;
using IsEmriBaslatma_WebUI.ModelView;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace IsEmriBaslatma_WebUI.Controllers
{
    public class UretimController : Controller
    {
        private readonly EFIsEmriManager _eFIsEmriManager = new EFIsEmriManager();
        private readonly EFIsEmriKalemleriManager _eFIsEmriKalemleriManager = new EFIsEmriKalemleriManager();
        private readonly EFSapmaManager _eFSapmaManager = new EFSapmaManager();
        private readonly SAPManeger _Sapmegar = new SAPManeger();

        // GET:  Uretim
        private List<IS_EMRI_KALEMLERI> Is_Emri_Verisi_Cekilmesi()
        {
            KULLANICILAR kullanicilar = (KULLANICILAR)Session["user"];
            string isEmriNumber = (string)Session["isEmriNumber"];
            bool Tekli_Coklu = true;
            string vardiyaBilgisi = (string)Session["VardiyaBilgisi"];
            SAPManeger sAPManeger = new SAPManeger();
            //IS EMRİ NUMARASI SQL DATA VAR MI 
            if (!string.IsNullOrEmpty(isEmriNumber))
            {
                // İş emri numarasının uzunluğunu kontrol ediyoruz
                if (isEmriNumber.Length < 12)
                {
                    // Eğer iş emri numarası 5 karakterden kısa ise önüne sıfır ekliyoruz
                    int eksikKarakterSayisi = 12 - isEmriNumber.Length;
                    isEmriNumber = isEmriNumber.PadLeft(12, '0');
                }
                int kontrol = _eFIsEmriManager.getWhereIsEmiNoToID(isEmriNumber);
                string status = _eFIsEmriManager.getWhereIsEmiNoToSTATU(isEmriNumber);
                if (kontrol == 0 && Session["is_emri_kalemleri"] == null)
                {
                    if (status == "Tamamlandi" || status == null)
                    {
                        IS_EMIRLERI isEmirleriBilgileri = sAPManeger.getFistIsEmirleri(isEmriNumber, Tekli_Coklu);
                        isEmirleriBilgileri.OLUSTURAN_ID = kullanicilar.ID;
                        Session["is_emirleri"] = isEmirleriBilgileri;
                        if (isEmirleriBilgileri.URUN_ADI == null)
                        {
                            TempData["IsEmriBulunamadi"] = "Girilen veriye ait bir iş emri bulunamadı.";
                            return null; ;
                        }
                        int IS_EMRI_ID = _eFIsEmriManager.getWhereIsEmiNoToID(isEmriNumber);
                        //2- SAP TARAFINDAN VERİLER ÇEKİLECEK.
                        //SAP İS EMİRLERİ LİSTESİ ÇEKİLDİ. 
                        List<IS_EMRI_KALEMLERI> is_emri_kalemleri = sAPManeger.getAllSap(isEmriNumber, Tekli_Coklu, IS_EMRI_ID);
                        int otoArtir = 1;
                        foreach (IS_EMRI_KALEMLERI item in is_emri_kalemleri)
                        {

                            item.SAPMA_DURUM = false;
                            item.ID = otoArtir;
                            otoArtir++;
                        }
                        //foreach (IS_EMRI_KALEMLERI item in is_emri_kalemleri)
                        //{
                        //    item.SAPMA_DURUM=false;
                        //}
                        //SAP GELEN İŞ EMİRLERİ SIRAYLA IS_EMRİ_KALEMLERİ TABLOSUNA YAZDIRDIM.
                        Session["is_emri_kalemleri"] = is_emri_kalemleri;

                        // 3- SAP GELEN VERİLER DATADAN ÇEKİLEREK VİEW GÖNDERİLDİ.
                        // ?? HIZ KAZANMAK İÇİN SAP GELEN LİST DİREK VİEW GONDERİLEBİLDİ AMA POPUP İÇİN DATADAN ID IHTIYACIMIZ VAR
                        ViewBag.YeniIs_EMRI_KALEMI = IS_EMRI_ID;
                        ViewBag.SiparisBilgileri = _eFIsEmriManager.getAllByIsEmri(IS_EMRI_ID);
                        //ViewBag.MalzemeListesi = sAPManeger.getAllMalzeme();
                        return (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                    }
                    else
                    {
                        //SQL DATALAR VARSA GÖRÜNTÜLENECEK
                        //getall where exprossion bulunamadı araştır tum listeden where yapma !!
                        int IS_EMRI_ID = _eFIsEmriManager.getIsEmri(isEmriNumber);
                        ViewBag.SiparisBilgileri = _eFIsEmriManager.getAllByIsEmri(IS_EMRI_ID);
                        ViewBag.YeniIs_EMRI_KALEMI = IS_EMRI_ID;
                        //ViewBag.MalzemeListesi = sAPManeger.getAllMalzeme();
                        if (Session["is_emri_kalemleri"] == null)
                        {
                            List<IS_EMRI_KALEMLERI> val = _eFIsEmriKalemleriManager.getAll().Where(X => X.is_EMRI_ID == IS_EMRI_ID).ToList();
                            Session["is_emri_kalemleri"] = (List<IS_EMRI_KALEMLERI>)val;
                            return (val);
                        }
                        else
                        {

                            var a = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                            // hatayı çöz
                            return (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                        }
                    };
                }

            }
            else
            {
                return (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
            }
            return (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];

        }

        private bool Is_Emri_Sapma_Durumu(int id, bool durum)
        {
            List<IS_EMRI_KALEMLERI> is_emri_detay_model = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
            IS_EMRI_KALEMLERI value = is_emri_detay_model.FirstOrDefault(x => x.ID == id);
            value.SAPMA_DURUM = durum;
            //value.BOLD = "X";
            try
            {
                _eFIsEmriKalemleriManager.update(value);
            }
            catch (System.Exception)
            {
            }
            Session["is_emri_kalemleri"] = is_emri_detay_model;
            return true;
        }
        [HttpGet]
        public ActionResult IsEmriKalemleri()
        {
            List<IS_EMRI_KALEMLERI> valueList = Is_Emri_Verisi_Cekilmesi()?.ToList();
            ViewBag.SiparisBilgileri = _eFIsEmriManager.getAllByIsEmri((int)valueList.FirstOrDefault().is_EMRI_ID);
            return valueList.Count > 0 ? View(valueList) : (ActionResult)RedirectToAction("Home", "IsEmriBaslatma");
        }

        [HttpPost]
        public ActionResult IsEmriKalemleri(string isEmriNumber, bool Tekli_Coklu, string vardiyaBilgisi)
        {
            KULLANICILAR kullanicilar = (KULLANICILAR)Session["user"];

            if (Session["isEmriNumber"] != null)
            {
                if (Session["isEmriNumber"].ToString() != isEmriNumber)
                {
                    Session["is_emri_kalemleri"] = null;
                }
            }
            Session["isEmriNumber"] = isEmriNumber;
            Session["Tekli_Coklu"] = Tekli_Coklu;
            Session["VardiyaBilgisi"] = vardiyaBilgisi;
            List<IS_EMRI_KALEMLERI> valueList = null;
            try
            {
                if (isEmriNumber.Length < 12)
                {
                    // Eğer iş emri numarası 5 karakterden kısa ise önüne sıfır ekliyoruz
                    int eksikKarakterSayisi = 12 - isEmriNumber.Length;
                    isEmriNumber = isEmriNumber.PadLeft(12, '0');
                }
                IS_EMIRLERI isEmirleri = _eFIsEmriManager.getAll().FirstOrDefault(x => x.IS_EMRI_NO == isEmriNumber && x.STATU == "Devam Ediyor");
                if (isEmirleri != null)
                {
                    if (isEmirleri.OLUSTURAN_ID != kullanicilar.ID)
                    {
                        TempData["FarkliKullanici"] = "Bu iş emri numarası" + "  " + isEmirleri.KULLANICILAR.SICIL_NO + " Kullanıcısı tarafından devam etmektedir.";
                        return RedirectToAction("IsEmriBaslatma", "Home");
                    }
                    else
                    {

                        valueList = Is_Emri_Verisi_Cekilmesi().ToList();
                        return View(valueList);

                    }

                }

            }
            catch (System.Exception)
            {

            }


            valueList = Is_Emri_Verisi_Cekilmesi().ToList();
            return valueList.Count > 0 ? View(valueList) : (ActionResult)RedirectToAction("IsEmriBaslatma", "Home");
        }

        public ActionResult _isEmirleriDetayTablo()
        {
            return View();
        }
        public ActionResult _MalzemeFilter(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                List<Malzeme_Listesi> malzemeListesiModel = _Sapmegar.getAllMalzeme().Where(x => x.Malzeme_Metni.Contains(searchValue)).ToList();
                if (malzemeListesiModel.Count() > 0)
                {
                    return View(malzemeListesiModel);
                }
                else
                {
                    return null;
                }
            }
            return View();
        }

        public ActionResult _urunDetaylariPopup()
        {

            return View();
        }
        [HttpPost]
        public ActionResult urunDetaylariPopup(int is_emri_id, string urunAdi, string isKalemInputMikar, string sapmanedeni)
        {
            List<IS_EMRI_KALEMLERI> is_emri_kalemleri = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
            IS_EMRI_KALEMLERI update_is_emri = is_emri_kalemleri.FirstOrDefault(x => x.ID == is_emri_id);
            if (update_is_emri != null)
            {
                update_is_emri.ALTERNATIF_URUN = urunAdi;
                update_is_emri.SAPMA_NEDENI = sapmanedeni;
                update_is_emri.KULLANILAN_MIKTAR = isKalemInputMikar;
                Is_Emri_Sapma_Durumu(is_emri_id, true);
                return RedirectToAction("IsEmriKalemleri");
            }
            else
            {
                return null;
            }

            //if (!sapma_Listesi.Any(x => x.IS_EMRI_KALEM_ID == is_emri_id))
            //{
            //    SAPMA sapmanewValue = new SAPMA();
            //    sapmanewValue.IS_EMRI_KALEM_ID = is_emri_id;
            //    sapmanewValue.GERCEKLESEN_MIKTAR = isKalemInputMikar;
            //    sapmanewValue.ALTERNATIF_URUN = urunAdi;
            //    sapmanewValue.SAPMA_NEDENI = sapmanedeni;
            //    foreach (var item in is_emri_kalemleri.Where(x => x.ID == is_emri_id))
            //    {
            //        item.SAPMA.Add(sapmanewValue);
            //    }
            //    Is_Emri_Sapma_Durumu(is_emri_id, true);
            //}
            //else
            //{
            //    foreach (IS_EMRI_KALEMLERI item in is_emri_kalemleri.Where(x => x.ID == is_emri_id))
            //    {
            //        foreach (SAPMA val in item.SAPMA)
            //        {
            //            val.ALTERNATIF_URUN = urunAdi;
            //            val.GERCEKLESEN_MIKTAR = isKalemInputMikar;
            //            val.SAPMA_NEDENI = sapmanedeni;

            //        }
            //    }
            //    Is_Emri_Sapma_Durumu(is_emri_id, true);
            //}
        }

        [HttpPost]
        public JsonResult _kalemOnay(int id, string kulMiktar, bool checkedStatus)
        {
            try
            {
                // Retrieve the list from the session
                List<IS_EMRI_KALEMLERI> is_emri_kalem_list = Session["is_emri_kalemleri"] as List<IS_EMRI_KALEMLERI>;

                if (is_emri_kalem_list != null)
                {
                    // Find the specific item by ID
                    IS_EMRI_KALEMLERI is_emri_kalem = is_emri_kalem_list.FirstOrDefault(x => x.ID == id);

                    if (is_emri_kalem != null)
                    {
                        if (checkedStatus)
                        {
                            // Update the item properties
                            is_emri_kalem.KULLANILAN_MIKTAR = kulMiktar;
                            is_emri_kalem.ALTERNATIF_URUN = "";
                            is_emri_kalem.SAPMA_NEDENI = "";
                            is_emri_kalem.SAPMA_DURUM = false;
                        }
                        else
                        {
                            // Reset the item properties
                            is_emri_kalem.KULLANILAN_MIKTAR = "";
                            is_emri_kalem.ALTERNATIF_URUN = "";
                            is_emri_kalem.SAPMA_NEDENI = "";
                            is_emri_kalem.SAPMA_DURUM = false;
                        }

                        // Update the individual item
                        if (_eFIsEmriKalemleriManager.update(is_emri_kalem) > 0)
                        {
                            return Json(new { success = true, message = "İşlem başarılı." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Güncelleme başarısız." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Kayıt bulunamadı." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Session verisi bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                // Return error message
                return Json(new { success = false, message = "İşlem sırasında hata oluştu: " + ex.Message });
            }
        }



        private List<IS_EMRI_KALEMLERI> Is_Emri_Kalemleri_Kontrol()
        {
            if (Session["is_emri_kalemleri"] == null)
            {
                return null;
            }
            else
            {
                return (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
            }
        }

        [HttpPost]
        public ActionResult _urunDetaylariModel(int id)
        {
            // Veriyi veri tabanından veya başka bir kaynaktan alın.
            // Bu örnekte, basit bir metin döndürüyoruz.
            List<IS_EMRI_KALEMLERI> listValue = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
            return View(listValue.FirstOrDefault(x => x.ID == id));
        }


        public ActionResult kayit()
        {
            IS_EMIRLERI isEmri = (IS_EMIRLERI)Session["is_emirleri"];
            List<IS_EMRI_KALEMLERI> isEmriNumber = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];


            if (_eFIsEmriManager.getAll().Any(x => x.IS_EMRI_ID == isEmriNumber.FirstOrDefault().is_EMRI_ID))
            {

                List<IS_EMRI_KALEMLERI> is_emri_kalemleri = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];

                foreach (IS_EMRI_KALEMLERI is_emirleri in is_emri_kalemleri)
                {
                    IS_EMRI_KALEMLERI save_is_emirleri = new IS_EMRI_KALEMLERI();
                    save_is_emirleri.is_EMRI_ID = isEmri.IS_EMRI_ID;
                    save_is_emirleri.ID = is_emirleri.ID;
                    save_is_emirleri.STOK_KODU = is_emirleri.STOK_KODU;
                    save_is_emirleri.MAL_GRUBU = is_emirleri.MAL_GRUBU;
                    save_is_emirleri.MIKTAR = is_emirleri.MIKTAR;
                    save_is_emirleri.URUN_BASINA_MIKTAR = is_emirleri.URUN_BASINA_MIKTAR;
                    //save_is_emirleri.BOLD = is_emirleri.BOLD;
                    save_is_emirleri.STOK_ISMI = is_emirleri.STOK_ISMI;
                    save_is_emirleri.IS_EMRI_NO = is_emirleri.IS_EMRI_NO;
                    save_is_emirleri.SAPMA_DURUM = is_emirleri.SAPMA_DURUM;
                    save_is_emirleri.ALTERNATIF_URUN = is_emirleri.ALTERNATIF_URUN;
                    save_is_emirleri.SAPMA_NEDENI = is_emirleri.SAPMA_NEDENI;
                    save_is_emirleri.KULLANILAN_MIKTAR = is_emirleri.KULLANILAN_MIKTAR;
                    save_is_emirleri.BOLD = is_emirleri.BOLD;
                    _eFIsEmriKalemleriManager.update(save_is_emirleri);


                    //Sapma tablosuna kayıt
                    //foreach (SAPMA sapma in is_emirleri.SAPMA)
                    //{
                    //    SAPMA upsapma = _eFSapmaManager.getByID(sapma.IS_EMRI_KALEM_ID);
                    //    if (upsapma == null)
                    //    {

                    //        SAPMA savesapma = new SAPMA();
                    //        savesapma.IS_EMRI_KALEM_ID = sapma.IS_EMRI_KALEM_ID;
                    //        savesapma.ALTERNATIF_URUN = sapma.ALTERNATIF_URUN;
                    //        savesapma.GERCEKLESEN_MIKTAR = sapma.GERCEKLESEN_MIKTAR;
                    //        savesapma.SAPMA_NEDENI = sapma.SAPMA_NEDENI;
                    //        savesapma.TARIH = System.DateTime.Now;
                    //        _eFSapmaManager.save(savesapma);
                    //        Is_Emri_Sapma_Durumu(sapma.IS_EMRI_KALEM_ID, true);

                    //    }
                    //    else
                    //    {

                    //        //savesapma.IS_EMRI_KALEM_ID = save_is_emirleri.ID;                           
                    //        upsapma.IS_EMRI_KALEM_ID = sapma.IS_EMRI_KALEM_ID;
                    //        upsapma.ALTERNATIF_URUN = sapma.ALTERNATIF_URUN;
                    //        upsapma.GERCEKLESEN_MIKTAR = sapma.GERCEKLESEN_MIKTAR;
                    //        upsapma.SAPMA_NEDENI = sapma.SAPMA_NEDENI;
                    //        upsapma.TARIH = System.DateTime.Now;
                    //        _eFSapmaManager.update(upsapma);
                    //    }
                    //}
                }

                Session["is_emri_kalemleri"] = null;
                return RedirectToAction("IsEmriKalemleri");
            }
            else
            {
                _eFIsEmriManager.Save(isEmri);
                List<IS_EMRI_KALEMLERI> is_emri_kalemleri = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                foreach (IS_EMRI_KALEMLERI is_emirleri in is_emri_kalemleri)
                {
                    IS_EMRI_KALEMLERI save_is_emirleri = new IS_EMRI_KALEMLERI();
                    save_is_emirleri.is_EMRI_ID = isEmri.IS_EMRI_ID;
                    save_is_emirleri.ID = is_emirleri.ID;
                    save_is_emirleri.KAYNAK = is_emirleri.KAYNAK;
                    save_is_emirleri.STOK_KODU = is_emirleri.STOK_KODU;
                    save_is_emirleri.MAL_GRUBU = is_emirleri.MAL_GRUBU;
                    save_is_emirleri.OLCU_BIRIMI = is_emirleri.OLCU_BIRIMI;
                    save_is_emirleri.MIKTAR = is_emirleri.MIKTAR;
                    save_is_emirleri.URUN_BASINA_MIKTAR = is_emirleri.URUN_BASINA_MIKTAR;
                    save_is_emirleri.STOK_ISMI = is_emirleri.STOK_ISMI;
                    save_is_emirleri.IS_EMRI_NO = is_emirleri.IS_EMRI_NO;
                    save_is_emirleri.SAPMA_DURUM = is_emirleri.SAPMA_DURUM;
                    save_is_emirleri.ALTERNATIF_URUN = is_emirleri.ALTERNATIF_URUN;
                    save_is_emirleri.KULLANILAN_MIKTAR = is_emirleri.KULLANILAN_MIKTAR;
                    save_is_emirleri.SAPMA_NEDENI = is_emirleri.SAPMA_NEDENI;
                    save_is_emirleri.BOLD = is_emirleri.BOLD;
                    isEmri.STATU = "Devam Ediyor";
                    _eFIsEmriManager.Update(isEmri);
                    _eFIsEmriKalemleriManager.save(save_is_emirleri);
                    //Sapma tablosu kayıt edilmesi
                    //foreach (SAPMA sapma in is_emirleri.SAPMA)
                    //{
                    //    SAPMA savesapma = new SAPMA();
                    //    savesapma.IS_EMRI_KALEM_ID = save_is_emirleri.ID;
                    //    savesapma.ALTERNATIF_URUN = sapma.ALTERNATIF_URUN;
                    //    savesapma.GERCEKLESEN_MIKTAR = sapma.GERCEKLESEN_MIKTAR;
                    //    savesapma.SAPMA_NEDENI = sapma.SAPMA_NEDENI;
                    //    savesapma.TARIH = System.DateTime.Now;
                    //    _eFSapmaManager.save(savesapma); //add vardı save sapma geliyor
                    //}

                }
                Session["is_emri_kalemleri"] = null;
                return RedirectToAction("IsEmriBaslatma", "Home");
            }
        }
        public ActionResult IsEmriBitir()
        {

            IS_EMIRLERI isEmri = (IS_EMIRLERI)Session["is_emirleri"];
            List<IS_EMRI_KALEMLERI> isEmriNumber = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];

            if (isEmriNumber.Any(x => x.KULLANILAN_MIKTAR == null &&  x.BOLD != "X"))
            {
                TempData["ErrorMessage"] = "Bu işlem bitirilemez. Boş alanlar var.";
                return RedirectToAction("IsEmriKalemleri"); // Hata mesajıyla birlikte aynı sayfaya yönlendir
            }
            else
            {
                if (_eFIsEmriManager.getAll().Any(x => x.IS_EMRI_ID == isEmriNumber.FirstOrDefault().is_EMRI_ID))
                {

                    List<IS_EMRI_KALEMLERI> is_emri_kalemleri = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                    foreach (IS_EMRI_KALEMLERI is_emirleri in is_emri_kalemleri)
                    {
                        IS_EMRI_KALEMLERI save_is_emirleri = new IS_EMRI_KALEMLERI();
                        save_is_emirleri.is_EMRI_ID = isEmri.IS_EMRI_ID;
                        save_is_emirleri.ID = is_emirleri.ID;
                        save_is_emirleri.STOK_KODU = is_emirleri.STOK_KODU;
                        save_is_emirleri.MAL_GRUBU = is_emirleri.MAL_GRUBU;
                        save_is_emirleri.MIKTAR = is_emirleri.MIKTAR;
                        save_is_emirleri.URUN_BASINA_MIKTAR = is_emirleri.URUN_BASINA_MIKTAR;
                        //save_is_emirleri.BOLD = is_emirleri.BOLD;
                        save_is_emirleri.STOK_ISMI = is_emirleri.STOK_ISMI;
                        save_is_emirleri.IS_EMRI_NO = is_emirleri.IS_EMRI_NO;
                        save_is_emirleri.SAPMA_DURUM = is_emirleri.SAPMA_DURUM;
                        save_is_emirleri.ALTERNATIF_URUN = is_emirleri.ALTERNATIF_URUN;
                        save_is_emirleri.SAPMA_NEDENI = is_emirleri.SAPMA_NEDENI;
                        save_is_emirleri.KULLANILAN_MIKTAR = is_emirleri.KULLANILAN_MIKTAR;
                        save_is_emirleri.BOLD = is_emirleri.BOLD;
                        isEmri.STATU = "Tamamlandi";
                        _eFIsEmriManager.Update(isEmri);
                        _eFIsEmriKalemleriManager.update(save_is_emirleri);

                        if (is_emirleri.SAPMA_DURUM == true)

                        {
                            SAPMA savesapma = new SAPMA();
                            savesapma.IS_EMRI_KALEM_ID = save_is_emirleri.ID;
                            savesapma.ALTERNATIF_URUN = save_is_emirleri.ALTERNATIF_URUN;
                            savesapma.GERCEKLESEN_MIKTAR = save_is_emirleri.KULLANILAN_MIKTAR;
                            savesapma.SAPMA_NEDENI = save_is_emirleri.SAPMA_NEDENI;
                            savesapma.TARIH = System.DateTime.Now;
                            _eFSapmaManager.save(savesapma); //add vardı save sapma geliyor
                        }
                        //Sapma tablosuna kayıt
                        //foreach (SAPMA sapma in is_emirleri.SAPMA)
                        //{
                        //    SAPMA upsapma = _eFSapmaManager.getByID(sapma.IS_EMRI_KALEM_ID);
                        //    if (upsapma == null)
                        //    {

                        //        SAPMA savesapma = new SAPMA();
                        //        savesapma.IS_EMRI_KALEM_ID = sapma.IS_EMRI_KALEM_ID;
                        //        savesapma.ALTERNATIF_URUN = sapma.ALTERNATIF_URUN;
                        //        savesapma.GERCEKLESEN_MIKTAR = sapma.GERCEKLESEN_MIKTAR;
                        //        savesapma.SAPMA_NEDENI = sapma.SAPMA_NEDENI;
                        //        savesapma.TARIH = System.DateTime.Now;
                        //        _eFSapmaManager.save(savesapma);
                        //        Is_Emri_Sapma_Durumu(sapma.IS_EMRI_KALEM_ID, true);

                        //    }
                        //    else
                        //    {

                        //        //savesapma.IS_EMRI_KALEM_ID = save_is_emirleri.ID;                           
                        //        upsapma.IS_EMRI_KALEM_ID = sapma.IS_EMRI_KALEM_ID;
                        //        upsapma.ALTERNATIF_URUN = sapma.ALTERNATIF_URUN;
                        //        upsapma.GERCEKLESEN_MIKTAR = sapma.GERCEKLESEN_MIKTAR;
                        //        upsapma.SAPMA_NEDENI = sapma.SAPMA_NEDENI;
                        //        upsapma.TARIH = System.DateTime.Now;
                        //        _eFSapmaManager.update(upsapma);
                        //    }
                        //}
                    }

                    Session["is_emri_kalemleri"] = null;
                    return RedirectToAction("IsEmriKalemleri");
                }
                else
                {
                    _eFIsEmriManager.Save(isEmri);
                    List<IS_EMRI_KALEMLERI> is_emri_kalemleri = (List<IS_EMRI_KALEMLERI>)Session["is_emri_kalemleri"];
                    foreach (IS_EMRI_KALEMLERI is_emirleri in is_emri_kalemleri)
                    {
                        IS_EMRI_KALEMLERI save_is_emirleri = new IS_EMRI_KALEMLERI();
                        save_is_emirleri.is_EMRI_ID = isEmri.IS_EMRI_ID;
                        save_is_emirleri.ID = is_emirleri.ID;
                        save_is_emirleri.KAYNAK = is_emirleri.KAYNAK;
                        save_is_emirleri.STOK_KODU = is_emirleri.STOK_KODU;
                        save_is_emirleri.MAL_GRUBU = is_emirleri.MAL_GRUBU;
                        save_is_emirleri.OLCU_BIRIMI = is_emirleri.OLCU_BIRIMI;
                        save_is_emirleri.MIKTAR = is_emirleri.MIKTAR;
                        save_is_emirleri.URUN_BASINA_MIKTAR = is_emirleri.URUN_BASINA_MIKTAR;
                        save_is_emirleri.STOK_ISMI = is_emirleri.STOK_ISMI;
                        save_is_emirleri.IS_EMRI_NO = is_emirleri.IS_EMRI_NO;
                        save_is_emirleri.SAPMA_DURUM = is_emirleri.SAPMA_DURUM;
                        save_is_emirleri.ALTERNATIF_URUN = is_emirleri.ALTERNATIF_URUN;
                        save_is_emirleri.KULLANILAN_MIKTAR = is_emirleri.KULLANILAN_MIKTAR;
                        save_is_emirleri.SAPMA_NEDENI = is_emirleri.SAPMA_NEDENI;
                        save_is_emirleri.BOLD = is_emirleri.BOLD;
                        isEmri.IS_EMRI_BITIS_TARIHI = System.DateTime.Now;
                        
                        isEmri.STATU = "Tamamlandi";
                        _eFIsEmriManager.Update(isEmri);
                        _eFIsEmriKalemleriManager.save(save_is_emirleri);
                        //Sapma tablosu kayıt edilmesi
                        if (is_emirleri.SAPMA_DURUM == true)
                       
                        {
                            SAPMA savesapma = new SAPMA();
                            savesapma.IS_EMRI_KALEM_ID = save_is_emirleri.ID;
                            savesapma.ALTERNATIF_URUN = save_is_emirleri.ALTERNATIF_URUN;
                            savesapma.GERCEKLESEN_MIKTAR = save_is_emirleri.KULLANILAN_MIKTAR;
                            savesapma.SAPMA_NEDENI = save_is_emirleri.SAPMA_NEDENI;
                            savesapma.TARIH = System.DateTime.Now;
                            _eFSapmaManager.save(savesapma); //add vardı save sapma geliyor
                        }

                    }


                
                   Session["is_emri_kalemleri"] = null;
                    return RedirectToAction("IsEmriBaslatma", "Home");
                }
            }
        }


        public ActionResult IsEmriGoruntuleme()
        {
           
            return View();

        }
        [HttpPost]
        public ActionResult IsEmriGoruntulemeKalemleri(string isEmriNumber, bool Tekli_Coklu)
        {

            SAPManeger sAPManeger = new SAPManeger();
            //IS EMRİ NUMARASI SQL DATA VAR MI 
            if (!string.IsNullOrEmpty(isEmriNumber))
            {
                // İş emri numarasının uzunluğunu kontrol ediyoruz
                if (isEmriNumber.Length < 12)
                {
                    // Eğer iş emri numarası 5 karakterden kısa ise önüne sıfır ekliyoruz
                    int eksikKarakterSayisi = 12 - isEmriNumber.Length;
                    isEmriNumber = isEmriNumber.PadLeft(12, '0');
                }
                int kontrol = _eFIsEmriManager.getWhereIsEmiNoToID(isEmriNumber);
                if (kontrol == 0)
                {
                    //SAP GELEN VERİLERİN ALINMA KISMI
                    // 1 - İS EMİRLERİ TABLOSUNU TABLODA OLMAYAN İS EMRİ NUMARASI KAYIT EDİLDİ.
                    IS_EMIRLERI isEmirleriBilgileri = sAPManeger.getFistIsEmirleri(isEmriNumber, Tekli_Coklu);
                    if (isEmirleriBilgileri.URUN_ADI == null)
                    {
                        TempData["IsEmriBulunamadi"] = "Girilen veriye ait bir iş emri bulunamadı.";
                        return RedirectToAction("IsEmriBaslatma");
                    }
                    _ = _eFIsEmriManager.Save(isEmirleriBilgileri);
                    int IS_EMRI_ID = _eFIsEmriManager.getWhereIsEmiNoToID(isEmriNumber);
                    //2- SAP TARAFINDAN VERİLER ÇEKİLECEK.
                    //SAP İS EMİRLERİ LİSTESİ ÇEKİLDİ. 
                    List<IS_EMRI_KALEMLERI> is_emri_kalemleri = sAPManeger.getAllSap(isEmriNumber, Tekli_Coklu, IS_EMRI_ID);
                    Session["is_Emri_Kalemleri"] = is_emri_kalemleri;
                    ////SAP GELEN İŞ EMİRLERİ SIRAYLA IS_EMRİ_KALEMLERİ TABLOSUNA YAZDIRDIM.
                    //foreach (IS_EMRI_KALEMLERI item in is_emri_kalemleri)
                    //{
                    //    item.SAPMA_DURUM = null;
                    //    _ = _eFIsEmriKalemleriManager.save(item);
                    //}
                    // 3- SAP GELEN VERİLER DATADAN ÇEKİLEREK VİEW GÖNDERİLDİ.
                    // ?? HIZ KAZANMAK İÇİN SAP GELEN LİST DİREK VİEW GONDERİLEBİLDİ AMA POPUP İÇİN DATADAN ID IHTIYACIMIZ VAR
                    ViewBag.YeniIs_EMRI_KALEMI = IS_EMRI_ID;
                    ViewBag.SiparisBilgileri = _eFIsEmriManager.getAllByIsEmri(IS_EMRI_ID);
                    //ViewBag.MalzemeListesi = sAPManeger.getAllMalzeme();
                    return View(is_emri_kalemleri);
                }
                else
                {
                    //SQL DATALAR VARSA GÖRÜNTÜLENECEK
                    //getall where exprossion bulunamadı araştır tum listeden where yapma !!
                    int IS_EMRI_ID = _eFIsEmriManager.getWhereIsEmiNoToID(isEmriNumber);
                    ViewBag.SiparisBilgileri = _eFIsEmriManager.getAllByIsEmri(IS_EMRI_ID);
                    ViewBag.YeniIs_EMRI_KALEMI = IS_EMRI_ID;
                    ViewBag.MalzemeListesi = sAPManeger.getAllMalzeme();
                    return View(_eFIsEmriKalemleriManager.getAll().Where(X => X.is_EMRI_ID == IS_EMRI_ID).ToList());
                };

            }
            int aa = Convert.ToInt32(isEmriNumber);
            return View(_eFIsEmriKalemleriManager.getAll().Where(X => X.is_EMRI_ID == aa).ToList());

        }
    }
}
