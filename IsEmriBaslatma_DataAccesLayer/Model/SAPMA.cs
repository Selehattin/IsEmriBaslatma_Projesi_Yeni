//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IsEmriBaslatma_DataAccesLayer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SAPMA
    {
        public int ID { get; set; }
        public int IS_EMRI_KALEM_ID { get; set; }
        public string GERCEKLESEN_MIKTAR { get; set; }
        public string ALTERNATIF_URUN { get; set; }
        public string ADET { get; set; }
        public string SAPMA_NEDENI { get; set; }
        public Nullable<System.DateTime> TARIH { get; set; }
        public string SAPMA_ONAY_DURUM { get; set; }
    
        public virtual IS_EMRI_KALEMLERI IS_EMRI_KALEMLERI { get; set; }
    }
}