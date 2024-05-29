using IsEmriBaslatma_BusinessLayer.EfManagers;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_WebUI.Helpers;
using IsEmriBaslatma_WebUI.ModelView;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace IsEmriBaslatma_WebUI.Controllers
{
    //[AuthFilter]
    public class HomeController : Controller
    {
        private readonly EFIsEmriManager _eFIsEmriManager = new EFIsEmriManager();
        private readonly EFIsEmriKalemleriManager _eFIsEmriKalemleriManager = new EFIsEmriKalemleriManager();
        private readonly EFSapmaManager _eFSapmaManager = new EFSapmaManager();
        public ActionResult Index()
        {

            return View();
        }
        //[HttpPost]
        public ActionResult IsEmriBaslatma()
        {
            if (TempData["FarkliKullanici"]!=null)
            {
                ViewBag.UserHata= (string)TempData["FarkliKullanici"];
            }
            Session["is_emri_kalemleri"] = null;
            ViewBag.IsEmriBulunamadi = TempData["IsEmriBulunamadi"];
            return View();
        }

        public ActionResult SessionReset()
        {

            Session["is_emri_kalemleri"] = null;
            return RedirectToAction("Index","Uretim");
        }

        [HttpPost]
        public ActionResult IsEmriDetay(string isEmriNumber, bool Tekli_Coklu)
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
                    Session["is_Emri_Kalemleri"]=is_emri_kalemleri;
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

        [HttpPost]
        public ActionResult YeniIsEmriKalemiEkleme(IS_EMRI_KALEMLERI iS_EMRI_KALEMLERI, SAPMA sAPMA)
        {
            if (_eFIsEmriKalemleriManager.save(iS_EMRI_KALEMLERI) > 0)
            {
                int IS_EMRI_ID = _eFIsEmriKalemleriManager.getAll().LastOrDefault().ID;
                sAPMA.IS_EMRI_KALEM_ID = IS_EMRI_ID;
                if (_eFSapmaManager.save(sAPMA) > 0)
                {
                    return Redirect("Index");
                }
            }
            return Redirect("Index");
        }

        public ActionResult IsEmriSiparisRapor()
        {

            SAPManeger _SAPManeger = new SAPManeger();
            return View(_SAPManeger.getAllCikti("000001000005"));
            //return View();
        }

        [HttpPost]
        public JsonResult MiktarSapma(string value, int isKalemId)
        {
            try
            {

                byte vardiya = Convert.ToByte(Session["VardiyaBilgisi"].ToString());
                IS_EMRI_KALEMLERI KALEMLERI = _eFIsEmriKalemleriManager.getByID(isKalemId);
                //List<SAPMA> sapmaa = _eFSapmaManager.getByWhereVardiya(KALEMLERI.ID, vardiya);
                //if (sapmaa.Count == 0)
                //{
                //    // SAPMA KAYDI YOKSA
                //    #region MyRegion
                //    double URUN_BASINA_MIKTAR = Convert.ToDouble(KALEMLERI.URUN_BASINA_MIKTAR);
                //    double MIKTAR = Convert.ToDouble(value);
                //    if (URUN_BASINA_MIKTAR == MIKTAR)
                //    {
                //        //SAPMA DURUMUNU FALSE YAPTIK
                //        KALEMLERI.SAPMA_DURUM = false;
                //        _ = _eFIsEmriKalemleriManager.update(KALEMLERI);
                //    }
                //    else
                //    {
                //        int olusturanID = 1; // Varsayılan olarak -1 veya herhangi bir değer atayabilirsiniz
                //        if (ControllerContext.HttpContext.Session["UserID"] != null)
                //        {
                //            olusturanID = Convert.ToInt32(ControllerContext.HttpContext.Session["UserID"]);
                //        }
                //        //SAPMA DURUMUNU TRUE YAPTIK
                //        KALEMLERI.SAPMA_DURUM = true;
                //        _ = _eFIsEmriKalemleriManager.update(KALEMLERI);

                //        //SAPMA sapma= EFAlternatifUrunlerManager.getByID(isKalemId);
                //        SAPMA sapma = new SAPMA
                //        {
                //            IS_EMRI_KALEM_ID = isKalemId,
                //            GERCEKLESEN_MIKTAR = value,
                //            TARIH = DateTime.Now,
                //        };
                //        _ = _eFSapmaManager.save(sapma);
                //    }
                //    // Burada gelen değerleri kullanarak gerekli işlemleri yapabilirsiniz.
                //    // Örneğin, gelen değerleri loglayabilir veya başka bir işlem gerçekleştirebilirsiniz.

                //    // Gelen değerleri kontrol etmek için birkaç örnek işlem yapalım:
                //    if (!string.IsNullOrEmpty(value))
                //    {
                //        System.Diagnostics.Debug.WriteLine("Gelen değer: " + value);
                //    }

                //    if (isKalemId > 0)
                //    {
                //        System.Diagnostics.Debug.WriteLine("Gelen isKalemId değeri: " + isKalemId);
                //    }

                //    // İşlem başarılı olduysa, uygun bir yanıt döndürün
                //    return Json(new { success = true });
                //    #endregion
                //}
                //else
                //{
                //    //SAPMA KAYDI VARSA
                //    SAPMA sapma = _eFSapmaManager.getByIDEx(KALEMLERI.ID);
                //    sapma.GERCEKLESEN_MIKTAR = value;
                //    sapma.TARIH = DateTime.Now;
                //    _ = _eFSapmaManager.update(sapma);
                //}

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // İşlem sırasında bir hata oluşursa, hata mesajını loglayın ve bir hata yanıtı döndürün
                System.Diagnostics.Debug.WriteLine("Hata oluştu: " + ex.Message);
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        public ActionResult MiktarSapma(List<string> isKalemleriID, List<string> isKalemInputMikar, List<string> checkDurum)
        {
            EFIsEmriKalemleriManager isEmriKalemleriManager = new EFIsEmriKalemleriManager();
            for (int i = 0; i < isKalemleriID.Count; i++)
            {
                int CislemKalemleriID = Convert.ToInt32(isKalemleriID[i]);
                if (isEmriKalemleriManager.getByID(CislemKalemleriID) != null)
                {
                    //IS_EMRI_VE_KALEMLERI oldData = isEmriKalemleriManager.getByID(CislemKalemleriID);
                    //if (oldData.URUN_BASI_MİKTAR == (isKalemInputMikar[i]))
                    //{
                    //    oldData.GerceklesenAdet = isKalemInputMikar[i];
                    //    oldData.SapmaDurumu = false;
                    //    oldData.IsEmriDurumu = false;
                    //    isEmriKalemleriManager.update(oldData);
                    //}
                    //else
                    //{
                    //    oldData.SapmaTarih = DateTime.Now;
                    //    oldData.SapmaDurumu = true;
                    //    oldData.GerceklesenAdet = isKalemInputMikar[i];
                    //    oldData.IsEmriDurumu = false;
                    //    isEmriKalemleriManager.update(oldData);

                    return View();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult YonlendirmeDeneme()
        {
            return RedirectToAction("Index");
        }

        public ActionResult urunDetayPopUp(int isEmriKalemleriID, string urunAdi, string aciklama, string sapmanedeni, string isKalemInputMikar)
        {
            #region IS EMRI KALEMLERI SAPMA DURMUNU TRUE OLARAK AYARLANMASI
            //IS EMRI KALEMLERI SAPMA DURMUNU TRUE OLARAK AYARLANMASI
            IS_EMRI_KALEMLERI isEmriKalemleriUpdate = _eFIsEmriKalemleriManager.getByID(isEmriKalemleriID);
            isEmriKalemleriUpdate.SAPMA_DURUM = true;
            _ = _eFIsEmriKalemleriManager.update(isEmriKalemleriUpdate);
            #endregion
            #region SAPMA DEGERİ KONTROL EDİLİYOR VARSA UPDATE EDİLİRYOR YOKSA YENİ KAYIT ALINIYOR.
            byte vardiya = Convert.ToByte(Session["VardiyaBilgisi"].ToString());
            ////List<SAPMA> sapmaDurumu = _eFSapmaManager.getByWhereVardiya(isEmriKalemleriID, vardiya);
            //if (sapmaDurumu.Count() == 0)
            //{
            //    //ISTENILEN IS EMRİ SAPMA LİSTESİNDE YOKSA
            //    //Kaydetme Yapıyoruz 
            //    _ = _eFSapmaManager.save(new SAPMA
            //    {
            //        IS_EMRI_KALEM_ID = isEmriKalemleriID,
            //        ALTERNATIF_URUN = urunAdi,
            //        GERCEKLESEN_MIKTAR = isKalemInputMikar,
                    
            //        TARIH = DateTime.Now,
            //        SAPMA_NEDENI = sapmanedeni,
            //        //OLUSTURAN_ID = kullanici.ID
            //        SAPMA_ONAY_DURUM = true,
            //    });
            //}
            //else
            //{
            //    //ISTENILEN IS EMRİ SAPMA LİSTESİNDE VARSA
            //    SAPMA sapma = _eFSapmaManager.getByIDEx(isEmriKalemleriID);
            //    sapma.ALTERNATIF_URUN = urunAdi;
            //    sapma.GERCEKLESEN_MIKTAR = isKalemInputMikar;
            //    sapma.TARIH = DateTime.Now;
            //    sapma.SAPMA_NEDENI = sapmanedeni;
            //    //sapma.OLUSTURAN_ID=kullanici.ID;
            //    _ = _eFSapmaManager.update(sapma);
            //}
            #endregion
            return Redirect("/home/index");
        }

        public ActionResult ArgeSapmaListesi()
        {

            EFIsEmriManager eFIsEmriManager = new EFIsEmriManager();
            EFIsEmriKalemleriManager EFIsEmriKalemleriManager = new EFIsEmriKalemleriManager();
            EFSapmaManager EFAlternatifUrunlerManager = new EFSapmaManager();

            //var query = @"SELECT s.[ID] AS [Sapma_ID]
            //                    , s.[IS_EMRI_KALEM_ID]
            //                    , s.[GERCEKLESEN_MIKTAR]
            //                    , s.[ALTERNATIF_URUN]
            //                    , s.[ACIKLAMA]
            //                    , s.[VARDIYA]
            //                    , s.[DURUM]
            //                    , s.[ADET]
            //                    , s.[SAPMA_NEDENI]
            //                    , s.[TARIH]
            //                    , s.[OLUSTURAN_ID]
            //                FROM [KLMSN_IS_BASLATMA].[dbo].[SAPMA] s
            //                INNER JOIN [KLMSN_IS_BASLATMA].[dbo].[IS_EMRI_KALEMLERI] k ON s.[IS_EMRI_KALEM_ID] = k.[ID]
            //                INNER JOIN [KLMSN_IS_BASLATMA].[dbo].[IS_EMIRLERI] e ON k.[is_EMRI_ID] = e.[IS_EMRI_ID]
            //                WHERE e.[IS_EMRI_NO] = @is_emri_no
            //                    AND s.[VARDIYA] = @vardiya";




            //var query = from ie in eFIsEmriManager.getAll()
            //            join iek in EFIsEmriKalemleriManager.getAll() on ie.IS_EMRI_ID equals iek.is_EMRI_ID
            //            group new { ie, iek } by new { ie.IS_EMRI_ID, ie.IS_EMRI_NO, ie.URUN_ADI, ie.URUN_KODU }
            //    into grouped
            //            select new
            //            {
            //                grouped.Key.id,
            //                SapmaTarih = grouped.Max(x => x.iek.SapmaTarih),
            //                Vardiya = grouped.Max(x => x.iek),
            //                IsEmriNO = grouped.Key.IS_EMRI_ID,
            //                UrunAdii = grouped.Key.URUN_ADI,
            //                UrunKoduu = grouped.Key.URUN_KODU
            //            };


            //var query = from s in EFAlternatifUrunlerManager.getAll()
            //join k in EFIsEmriKalemleriManager.getAll() on s.IS_EMRI_KALEM_ID equals k.ID
            //join e in eFIsEmriManager.getAll() on k.is_EMRI_ID equals e.IS_EMRI_ID
            //where e.IS_EMRI_NO == "000001011609" && s.VARDIYA == 2
            //group new { s, k, e } by new { e.IS_EMRI_ID, e.IS_EMRI_NO, e.URUN_ADI, e.URUN_KODU,s.TARIH,s.GERCEKLESEN_MIKTAR,s.VARDIYA} into grouped
            //select new
            //{
            //    grouped.Key.IS_EMRI_ID,
            //    grouped.Key.IS_EMRI_NO,
            //    SapmaTarih = grouped.Max(x => x.s.TARIH),
            //    Vardiya = grouped.Max(x => x.s.VARDIYA),
            //    UrunAdii = grouped.Key.URUN_ADI,
            //    UrunKoduu = grouped.Key.URUN_KODU,
            //    Miktar = grouped.Key.GERCEKLESEN_MIKTAR

            //};

            var query = from e in eFIsEmriManager.getAll()
                        select new
                        {
                            e.IS_EMRI_ID,
                            e.IS_EMRI_NO,
                            UrunAdii = e.URUN_ADI,
                            Is_Emri_No = e.IS_EMRI_NO,
                            seviye = e.COKLU_SEVIYE ?? e.TEKLI_SEVIYE,
                            UrunKoduu = e.URUN_KODU
                        };


            List<argeSapmaListesiModelView> modelsapma = new List<argeSapmaListesiModelView>();
            foreach (var item in query.ToList())
            {
                //DateTime spT = Convert.ToDateTime(item.SapmaTarih);
                //int isEmriNo = Convert.ToInt32(item.IS_EMRI_NO);
                //int vardiya = Convert.ToInt32(item.Vardiya);
                modelsapma.Add(new argeSapmaListesiModelView
                {
                    ID = item.IS_EMRI_ID.ToString(),
                    //TARIH = spT,
                    // VARDIYA = vardiya,

                    IS_EMRI_NO = item.IS_EMRI_NO,
                    SEVIYE = item.seviye,
                    URUN_ADI = item.UrunAdii,


                    //URUN_ADI= item.UrunAdii,
                    //GERCEKLESEN_MIKTAR = item.Miktar
                });




            }

            return View(modelsapma);
        }

        [HttpPost]
        public ActionResult SiparisCikti(string isEmriNumber)
        {
            return View();

        }


        //public ActionResult _argeSapmaListesiPopUpMenu(string isEmriNumber)
        //{
        //    EFIsEmriKalemleriManager isEmriKalemleriManager = new EFIsEmriKalemleriManager();
        //    IEnumerable<IsEmirleriKalemleri> isEmirleriKalemleris = isEmriKalemleriManager.getAll().Where(x => x.IS_EMRI_NO == isEmriNumber && x.SapmaDurumu == true);
        //    return View(isEmirleriKalemleris);
        //}

        //public ActionResult ArgeSapmaOnayListesi(string isEmriNumber = "1180792")
        //{
        //    int isEmirleriID;
        //    try
        //    {
        //        // İlgili iş emri datasına eriştim
        //        EFIsEmriManager isEmriManager = new EFIsEmriManager();
        //        IsEmirleri isEmirleriSb = isEmriManager.getAll().FirstOrDefault(x => x.IsEmriNo == isEmriNumber);
        //        //isEmirleriID = isEmirleriSb.IsEmriNo;
        //        ViewBag.isemirleriBilgileri = isEmirleriSb;
        //    }
        //    catch (Exception)
        //    {
        //        TempData["IsEmriBulunamadi"] = "İs emri bulunamadı.";
        //        return RedirectToAction("IsEmriBaslatma", "Home");
        //    }

        //    return View();
        //}
        [HttpPost]
        public ActionResult CallRFC(string stokKodu)
        {
            SAPManeger sAPManeger = new SAPManeger();
            // Aldığınız stok kodunu kullanarak RFC çağrısını gerçekleştirin
            // Örneğin: RFC çağrısı yap ve sonucu bir değişkende sakla
            // Sonucu işleyin ve istemciye geri dönün

            // Örnek: RFC çağrısı yapıldıktan sonra dönen veriyi işleyerek geri dönüş yapalım
            List<Resim> result = sAPManeger.getAllResim(stokKodu);
            return Json(new { success = true, result }); // Örnek bir başarı yanıtı
        }

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
                    parms.Add(RfcConfigParameters.Language, "TR");
                    parms.Add(RfcConfigParameters.PoolSize, "5");
                }

                return parms;
            }
        }
    }
}









