

using IsEmriBaslatma_DataAccesLayer.Model;
using System;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfInterface
{
    public interface IIsEmri
    {
        List<IS_EMIRLERI> getAll();
        IS_EMIRLERI getAllByIsEmri(int id);
        int getWhereIsEmiNoToID(string IS_EMRI_NO);
        string getWhereIsEmiNoToSTATU(string IS_EMRI_NO);
        int Save(IS_EMIRLERI T);
       
        
    }
}
