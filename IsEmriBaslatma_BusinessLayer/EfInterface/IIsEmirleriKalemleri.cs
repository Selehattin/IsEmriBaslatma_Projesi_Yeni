using IsEmriBaslatma_DataAccesLayer.Model;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfInterface
{
    public interface IIsEmirleriKalemleri
    {
        List<IS_EMRI_KALEMLERI> getAll();
        IS_EMRI_KALEMLERI getByID(int id);
        List<IS_EMRI_KALEMLERI> getByWhere(int IS_EMRI_ID);
        int update(IS_EMRI_KALEMLERI model);
        int save(IS_EMRI_KALEMLERI model);

    }
}
