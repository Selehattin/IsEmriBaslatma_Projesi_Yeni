using IsEmriBaslatma_BusinessLayer.EfManagers;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_WebUI.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace IsEmriBaslatma_WebUI.Controllers
{
    [AuthFilter]
    public class ArgeController : Controller
    {
        private readonly EFIsEmriManager _efIsEmriManager = new EFIsEmriManager();
        private readonly EFIsEmriKalemleriManager _efisemriKalemleriManager = new EFIsEmriKalemleriManager();
        private readonly EFSapmaManager _efsapmaManager = new EFSapmaManager();

        // GET: Arge
        public ActionResult ArgeListesi()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ArgeListesiDetayy(string isemrino)
        {
            return View(_efsapmaManager.getAll());

            //          // isemrino parametresine göre filtreleme yaparak ilgili veri setini elde etmek için gerekli işlemleri yapın.
            //var filteredData = efsapmaManager.getByIDEx(isemrino); // İş emri numarasına göre SAPMA nesnesi elde edin

            // return View(filteredData);; // Filtrelenmiş veri setini görünüme gönderin.
        }

        public ActionResult ArgeListesiSapmaSil(int id)
        {
            SAPMA sapma = _efsapmaManager.getByID(id);
            IS_EMRI_KALEMLERI isEmriKalemleriUp = sapma.IS_EMRI_KALEMLERI;
            isEmriKalemleriUp.SAPMA_DURUM = false;
            _ = _efisemriKalemleriManager.update(isEmriKalemleriUp);
            _ = _efsapmaManager.Delete(id);
            return RedirectToAction("ArgeListesiDetay", new { id = isEmriKalemleriUp.is_EMRI_ID });
        }

        [HttpGet]
        public ActionResult ArgeListesiDetay(int id)
        {


            // SAPMA listesi
            List<SAPMA> sAPMAs = new List<SAPMA>();

            // IS_EMRI_KALEMLERI tablosundan gelen id ile SAPMA tablosundaki karşılığı bul
            IEnumerable<IS_EMRI_KALEMLERI> isEmriKalemleri = _efisemriKalemleriManager.getByWhere(id).Where(x => x.SAPMA_DURUM == true);

            // Her IS_EMRI_KALEMLERI kaydı için SAPMA tablosundaki karşılığına bakarak SAPMA listesine ekle
            foreach (IS_EMRI_KALEMLERI isEmriKalem in isEmriKalemleri)
            {
                // IS_EMRI_KALEMLERI tablosundan gelen id ile SAPMA tablosundaki kaydı bul
                SAPMA sapma = _efsapmaManager.getByIDEx(isEmriKalem.ID);

                // Eğer SAPMA kaydı null değilse SAPMA listesine ekle
                if (sapma != null)
                {
                    sAPMAs.Add(sapma);
                }
            }

            // SAPMA listesindeki kayıtları alternatif ürün, adet ve sapma nedenine göre grupla
            var groupedItems = sAPMAs.GroupBy(x => new { x.ALTERNATIF_URUN, x.ADET, x.SAPMA_NEDENI });

            // Her gruptan bir öğe seçerek SAPMA listesini güncelle
            //sAPMAs = groupedItems.Select(group => group.First()).ToList();

            sAPMAs = groupedItems.Select(group => group.OrderByDescending(x => x.TARIH).First()).ToList();



            return View(sAPMAs);

            //List<SAPMA> sAPMAs = new List<SAPMA>();
            // IEnumerable<IS_EMRI_KALEMLERI> a=_efisemriKalemleriManager.getByWhere(id).Where(x => x.SAPMA_DURUM == true);


            // foreach (var item in a)
            // {
            //   sAPMAs.Add(_efsapmaManager.getByIDEx(item.ID));
            // }
            // return View(sAPMAs);
        }

        [HttpGet]

        public ActionResult ArgeListesiDetays()
        {

            return View();
        }



        //[HttpPost]
        //public ActionResult FilterByVardiya(byte vardiya, string tarih)
        //{
        //    List<SAPMA> filteredData = new List<SAPMA>();
        //    if (vardiya != null && tarih != null)
        //    {
        //        //filteredData = efsapmaManager.getAll().Where(x => x.VARDIYA == vardiya && x.TARIH==tarih).ToList();
        //    }
        //    if (vardiya != null)
        //    {
        //        //filteredData = _efsapmaManager.getAll().Where(x => x.VARDIYA == vardiya).ToList();
        //    }
        //    if (tarih != null)
        //    {
        //        //filteredData = efsapmaManager.getAll().Where(x => x.TARIH == tarih).ToList();
        //    }
        //    // Seçilen vardiya numarasına göre verileri filtrele

        //    return PartialView("_ArgeSapmaListesiTablosu", filteredData); // Filtrelenmiş verileri PartialView olarak geri döndür
        //}
    }


    // public ActionResult ExportToExcel()
    //{
    //    var data = GetData(); // Retrieve your data from the database or wherever it is stored

    //    using (ExcelPackage package = new ExcelPackage())
    //    {
    //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
    //        // Add headers
    //        worksheet.Cells[1, 1].Value = "IS_EMRI_NO";
    //        worksheet.Cells[1, 2].Value = "STOK_ISMI";
    //        worksheet.Cells[1, 3].Value = "STOK_KODU";
    //        worksheet.Cells[1, 4].Value = "URUN_BASINA_MIKTAR";
    //        worksheet.Cells[1, 5].Value = "ALTERNATIF_URUN";
    //        worksheet.Cells[1, 6].Value = "GERCEKLESEN_MIKTAR";
    //        worksheet.Cells[1, 7].Value = "VARDIYA";
    //        worksheet.Cells[1, 8].Value = "SAPMA_NEDENI";
    //        worksheet.Cells[1, 9].Value = "TARIH";
    //        worksheet.Cells[1, 10].Value = "SICIL_NO";

    //        // Add data
    //        int row = 2;
    //        foreach (var item in data)
    //        {
    //            worksheet.Cells[row, 1].Value = item.IS_EMRI_KALEMLERI.IS_EMRI_NO;
    //            worksheet.Cells[row, 2].Value = item.IS_EMRI_KALEMLERI.STOK_ISMI;
    //            worksheet.Cells[row, 3].Value = item.IS_EMRI_KALEMLERI.STOK_KODU;
    //            worksheet.Cells[row, 4].Value = item.IS_EMRI_KALEMLERI.URUN_BASINA_MIKTAR;
    //            worksheet.Cells[row, 5].Value = item.ALTERNATIF_URUN;
    //            worksheet.Cells[row, 6].Value = item.GERCEKLESEN_MIKTAR;
    //            worksheet.Cells[row, 7].Value = item.VARDIYA;
    //            worksheet.Cells[row, 8].Value = item.SAPMA_NEDENI;
    //            worksheet.Cells[row, 9].Value = item.TARIH;
    //            worksheet.Cells[row, 10].Value = item.KULLANICILAR.SICIL_NO;
    //            row++;
    //        }

    //        var stream = new MemoryStream();
    //        package.SaveAs(stream);
    //        string fileName = "ArgeListesi.xlsx";
    //        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    //        stream.Position = 0;
    //        return File(stream, contentType, fileName);
    //    }
    //}




}