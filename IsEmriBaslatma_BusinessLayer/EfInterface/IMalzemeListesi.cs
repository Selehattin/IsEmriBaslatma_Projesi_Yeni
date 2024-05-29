using IsEmriBaslatma_DataAccesLayer.Model;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfInterface
{
    public interface IMalzemeListesi
    {
         List<IS_EMRI_KALEMLERI> getByWhereSearch(string searchValue);
    }
}
