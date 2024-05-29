using System;

namespace IsEmriBaslatma_WebUI.Models
{
    public class IsEmriViewModel
    {
        public int IsEmriId { get; set; }
        public string StokKodu { get; set; }
        public string StokIsmi { get; set; }
        public string GerceklesenMiktar { get; set; }
        public string Aciklama { get; set; }
        public string AlternatifUrun { get; set; }
        public byte? Vardiya { get; set; }
        public string SapmaNedeni { get; set; }
        public DateTime? Tarih { get; set; }
        public int? OlusturanId { get; set; }
    }

}