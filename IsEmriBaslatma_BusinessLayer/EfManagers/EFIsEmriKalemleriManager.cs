using IsEmriBaslatma_BusinessLayer.EfInterface;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_EntitiyLayer.Repository;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfManagers
{
    public class EFIsEmriKalemleriManager : IIsEmirleriKalemleri
    {
        private readonly RepositoryDal<IS_EMRI_KALEMLERI> efIsEmriKalemleri = new RepositoryDal<IS_EMRI_KALEMLERI>();

        public IS_EMRI_KALEMLERI getByID(int id)
        {
            return efIsEmriKalemleri.Get(id);
        }

        public List<IS_EMRI_KALEMLERI> getAll()
        {
            return efIsEmriKalemleri.GetAll();
        }

        public int update(IS_EMRI_KALEMLERI model)
        {
            return efIsEmriKalemleri.Update(model);
           
        }

        public int save(IS_EMRI_KALEMLERI model)
        {

            return efIsEmriKalemleri.Save(model);
        }

        public List<IS_EMRI_KALEMLERI> getByWhere(int IS_EMRI_ID)
        {
            return efIsEmriKalemleri.GetWhere(x => x.is_EMRI_ID == IS_EMRI_ID);
        }

        public IS_EMRI_KALEMLERI getByWhereIS_Emri_No(int IS_EMRI_ID)
        {
            return efIsEmriKalemleri.GetFistOrDefault(x => x.is_EMRI_ID == IS_EMRI_ID);
        }
    }
}
