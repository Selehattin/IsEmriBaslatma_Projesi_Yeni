using IsEmriBaslatma_DataAccesLayer.Model;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfInterface
{
    public interface IAccounts
    {
        List<KULLANICILAR> getAll();
        KULLANICILAR get(int id);
        int Save(KULLANICILAR T);
        int update(KULLANICILAR model);
    }
}
