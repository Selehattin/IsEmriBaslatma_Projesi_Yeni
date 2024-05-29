using IsEmriBaslatma_BusinessLayer.EfInterface;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_EntitiyLayer.Repository;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfManagers
{
    public class EFLoginManager : IAccounts
    {
        private readonly RepositoryDal<KULLANICILAR> efAccounts = new RepositoryDal<KULLANICILAR>();

        public KULLANICILAR get(int id)
        {
            return efAccounts.Get(id);
        }
        public List<KULLANICILAR> getAll()
        {
            return efAccounts.GetAll();
        }

        public int Save(KULLANICILAR T)
        {
            return efAccounts.Save(T);
        }

        public int update(KULLANICILAR model)
        {
            return efAccounts.Update(model);
        }
    }
}
