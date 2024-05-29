using IsEmriBaslatma_DataAccesLayer.Model;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfInterface
{
    public interface ISapma
    {
        List<SAPMA> getAll();
        int update(SAPMA model);
        List<SAPMA> getByWhere(int IS_EMRI_ID);
        
        int save(SAPMA model);
        SAPMA getByID(int id);
        SAPMA getByIDEx(int IS_EMRI_ID);
        int Delete(int sapmaID);
    }
}
