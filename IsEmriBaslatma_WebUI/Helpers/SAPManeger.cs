using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_WebUI.ModelView;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Web.WebPages;

namespace IsEmriBaslatma_WebUI.Helpers
{
    public class SAPManeger
    {
        private readonly List<IS_EMRI_KALEMLERI> iS_EMRI_KALEMLERI = new List<IS_EMRI_KALEMLERI>();
        public List<IS_EMRI_KALEMLERI> getAllSap(string IS_EMRI_NO, bool Tekli_Coklu, int IS_EMRI_ID)
        {

            string tekSeviye = null;
            string cokSeviye = null;
            if (Tekli_Coklu)
            {
                tekSeviye = "X";
                cokSeviye = "";
            }
            else
            {
                tekSeviye = "";
                cokSeviye = "X";
            }

            try
            {
                ECCDestinationConfig cfg = new ECCDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(cfg);
                RfcDestination dest = RfcDestinationManager.GetDestination("rfc");

                RfcRepository repo = dest.Repository;

                IRfcFunction fnpush = repo.CreateFunction("ZPP_FM_012_RFC");
                IRfcTable table = fnpush.GetTable("IT_AUFNR");

                table.Append();
                table.SetValue("AUFNR", IS_EMRI_NO);
              
                fnpush.SetValue("IV_COK_SEVIYE", "X");
                //fnpush.SetValue("IV_TEK_SEVIYE", Tekli_Coklu );

                fnpush.Invoke(dest);

                IRfcTable it_table = fnpush.GetTable("T_ITEMS");

                // İş emri kalemlerinin listesini al

                foreach (IRfcStructure row in it_table)
                {
                    iS_EMRI_KALEMLERI.Add(new IS_EMRI_KALEMLERI
                    {
                        is_EMRI_ID = IS_EMRI_ID,
                        IS_EMRI_NO = row.GetString("IS_EMRI_NO"),
                        KAYNAK = row.GetString("KAYNAK"),
                        STOK_KODU = row.GetString("STOK_KODU"),
                        STOK_ISMI = row.GetString("STOK_ISMI"),
                        MAL_GRUBU = row.GetString("MAL_GRUBU"),
                        OLCU_BIRIMI = row.GetString("OLCU_BIRIMI"),
                        MIKTAR = row.GetString("MIKTAR"),
                        URUN_BASINA_MIKTAR = row.GetString("URUN_BASI_MIKTAR"),
                        BOLD = row.GetString("BOLD"),
                        
                    });
                }

                RfcSessionManager.EndContext(dest);
                RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır
                return iS_EMRI_KALEMLERI;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IS_EMIRLERI getFistIsEmirleri(string IS_EMRI_NO, bool Tekli_Coklu)
        {

            string tekSeviye = null;
            string cokSeviye = null;
            if (Tekli_Coklu)
            {
                tekSeviye = "X";
                cokSeviye = "";
            }
            else
            {
                tekSeviye = "";
                cokSeviye = "X";
            }
            IS_EMIRLERI isEmirleriBilgileri = new IS_EMIRLERI();
            ECCDestinationConfig cfg = new ECCDestinationConfig();
            RfcDestinationManager.RegisterDestinationConfiguration(cfg);
            RfcDestination dest = RfcDestinationManager.GetDestination("rfc");
            RfcRepository repo = dest.Repository;
            IRfcFunction fnpush = repo.CreateFunction("ZPP_ILK_URUN");

            fnpush.SetValue("AUFNR", IS_EMRI_NO);


            fnpush.Invoke(dest);

            IRfcTable urun_table = fnpush.GetTable("T_ZPPILKURUN");

            // Iterate through the RFC table and map its data to Siparis_Bilgileri objects
            foreach (IRfcStructure row in urun_table)
            {
                isEmirleriBilgileri.IS_EMRI_NO = row.GetString("AUFNR");
                isEmirleriBilgileri.URUN_ADI = row.GetString("MAKTX");
                isEmirleriBilgileri.URUN_KODU = row.GetString("STLBEZ");
                isEmirleriBilgileri.Toplam_Siparis_Miktari = row.GetString("GAMNG");
                isEmirleriBilgileri.Temel_Olcu_Birimi = row.GetString("GMEIN");
                isEmirleriBilgileri.TARIH = DateTime.Now.ToString("dd-MM-yyyy");
                isEmirleriBilgileri.TEKLI_SEVIYE = tekSeviye;
                isEmirleriBilgileri.COKLU_SEVIYE = cokSeviye;

               
            }

            RfcSessionManager.EndContext(dest);
            RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır
            return isEmirleriBilgileri;
        }
        public List<Malzeme_Listesi> getAllMalzeme()
        {

            List<Malzeme_Listesi> malzeme_Listesi = new List<Malzeme_Listesi>();
            RfcDestination dest = null;

            ECCDestinationConfig cfg = new ECCDestinationConfig();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(cfg);
            }
            catch (Exception)
            {

                RfcSessionManager.EndContext(dest);
                RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır.
            }
            dest = RfcDestinationManager.GetDestination("rfc");

            RfcRepository repo = dest.Repository;
            IRfcFunction fnpush = repo.CreateFunction("ZMM_043_MATERIAL_LIST");


            fnpush.Invoke(dest);

            IRfcTable malzeme_table = fnpush.GetTable("IT_MAT_LIST");

            // Iterate through the RFC table and map its data to Siparis_Bilgileri objects
            foreach (IRfcStructure row in malzeme_table)
            {
                malzeme_Listesi.Add(new Malzeme_Listesi
                {
                    Malzeme_No = row.GetString("MATNR"),
                    Malzeme_Metni = row.GetString("MAKTX")
                });
            }

            RfcSessionManager.EndContext(dest);
            RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır
            return malzeme_Listesi;
        }
        public List<Resim> getAllResim(string stokKodu)
        {
            List<Resim> resimler = new List<Resim>();

            ECCDestinationConfig cfg = null;
            RfcDestination dest = null;

            try
            {
                cfg = new ECCDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(cfg);
                dest = RfcDestinationManager.GetDestination("rfc");

                RfcRepository repo = dest.Repository;
                IRfcFunction fnpush = repo.CreateFunction("ZPP_MALZEME_RESIM");
                fnpush.SetValue("IV_NESNEANAHTARI", " ");

                fnpush.Invoke(dest);

                string evFilename = fnpush.GetString("EV_FILENAME"); // EV_FILENAME parametresini IRfcTable yerine dize olarak al

                // Dizeyi işleyerek Resim listesine ekle
                if (!string.IsNullOrEmpty(evFilename))
                {
                    resimler.Add(new Resim { Dosya_Yolu = evFilename });
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun işlemler yapılabilir, örneğin loglama veya hata mesajı döndürme
                Console.WriteLine("Hata: " + ex.Message);
            }
            finally
            {
                if (dest != null)
                {
                    RfcSessionManager.EndContext(dest);
                    RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır
                }
            }

            return resimler; // Tüm kod yollarında bir değer döndürmek için List<Resim> döndür
        }


        public List<Siparis_Cikti> getAllCikti(string IS_EMRI_NO)


        {
            List<Siparis_Cikti> siparis_cikti = new List<Siparis_Cikti>();
            try
            {
                ECCDestinationConfig cfg = new ECCDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(cfg);
                RfcDestination dest = RfcDestinationManager.GetDestination("rfc");

                RfcRepository repo = dest.Repository;

                IRfcFunction fnpush = repo.CreateFunction("ZPP_SIPARIS_CIKTISI");
                IRfcTable table = fnpush.GetTable("AUFNR");

                fnpush.SetValue("AUFNR", IS_EMRI_NO);

                fnpush.Invoke(dest);

                IRfcTable Cikti = fnpush.GetTable("T_ZPPSIPARIS");

                // İş emri kalemlerinin listesini al

                foreach (IRfcStructure row in Cikti)
                {
                    siparis_cikti.Add(new Siparis_Cikti
                    {
                        Siparis_Veren_Cari_No = row.GetString("KUNNR"),
                        Siparis_Veren_Cari_Isim = row.GetString("NAME1") + " " + row.GetString("NAME2"),
                        Musteri_Grup = row.GetString("KDGRP"),
                        Mali_Teslim_Alan_Isim = row.GetString("LAND1"),
                        Siparis_No = row.GetString("VBELN"),
                        Siparis_Kalem_No = row.GetString("POSNR"),
                        Siparis_Tarihi = row.GetString("ERDAT").AsDateTime(),
                        Malzeme_No = row.GetString("MATNR"),
                        Orjinal_Kod_Tanimi = row.GetString("ARKTX"),
                        Siparis_Miktari = row.GetString("KWMENG"),
                        Sertifika_Ulke = row.GetString("ZZULKE"),
                        Sertifika = row.GetString("ZZSERTIFIKA"),
                        YP_Gonderim_Durumu = row.GetString("KVGR4"),
                        Orj_Referans_Kod = row.GetString("MVGR4"),
                        Siparis_Yaratan_Kullanici = row.GetString("ERNAM"),

                    });
                }
                RfcSessionManager.EndContext(dest);
                RfcDestinationManager.UnregisterDestinationConfiguration(cfg); // SAP istemcisini sonlandır
                return siparis_cikti;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}