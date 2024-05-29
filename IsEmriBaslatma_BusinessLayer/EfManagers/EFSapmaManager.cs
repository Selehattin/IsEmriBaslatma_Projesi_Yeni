using IsEmriBaslatma_BusinessLayer.EfInterface;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_EntitiyLayer.Repository;
using System.Collections.Generic;
using System.Linq;

namespace IsEmriBaslatma_BusinessLayer.EfManagers
{
    public class EFSapmaManager : ISapma
    {
        private readonly RepositoryDal<SAPMA> EfSapma = new RepositoryDal<SAPMA>();
        public List<SAPMA> getAll()
        {
            return EfSapma.GetAll();
        }
        public SAPMA getByID(int id)
        {
            return EfSapma.Get(id);
        }
        public SAPMA getByIDEx(int IS_EMRI_ID)
        {
            return EfSapma.GetFistOrDefault(x => x.IS_EMRI_KALEM_ID == IS_EMRI_ID);
        }
        public List<SAPMA> getByWhereID(int IS_EMRI_ID)
        {
            return EfSapma.GetWhere(x => x.IS_EMRI_KALEM_ID == IS_EMRI_ID);
        }
        public List<SAPMA> getByWhere(int IS_EMRI_ID)
        {
            return EfSapma.GetWhere(x => x.IS_EMRI_KALEM_ID == IS_EMRI_ID).ToList();
        }
        
        public int save(SAPMA model)
        {
            return EfSapma.Save(model);
        }
        public int Delete(int sapmaID)
        {
            SAPMA sapma = EfSapma.Get(sapmaID);
            return EfSapma.Remove(sapma);
        }

        public int update(SAPMA model)
        {
            return EfSapma.Update(model);
        }
    }
}
