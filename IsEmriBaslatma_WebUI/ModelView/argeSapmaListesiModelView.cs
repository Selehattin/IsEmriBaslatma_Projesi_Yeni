using System;

namespace IsEmriBaslatma_WebUI.ModelView
{
    public class argeSapmaListesiModelView
    {
        public string ID { get; set; }
        public string IS_EMRI_NO { get; set; }
        public DateTime TARIH { get; set; }
        public int VARDIYA { get; set; }
        public string SEVIYE { get; set; }
        public string GERCEKLESEN_MIKTAR { get; set; }
        public string URUN_ADI { get; set; }
    }
}