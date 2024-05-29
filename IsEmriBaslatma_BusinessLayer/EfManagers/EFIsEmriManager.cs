using IsEmriBaslatma_BusinessLayer.EfInterface;
using IsEmriBaslatma_DataAccesLayer.Model;
using IsEmriBaslatma_EntitiyLayer.Repository;
using System;
using System.Collections.Generic;

namespace IsEmriBaslatma_BusinessLayer.EfManagers
{
    public class EFIsEmriManager : IIsEmri
    {
        private readonly RepositoryDal<IS_EMIRLERI> efIsEmrii = new RepositoryDal<IS_EMIRLERI>();
        public List<IS_EMIRLERI> getAll()
        {
            return efIsEmrii.GetAll();
        }

        public IS_EMIRLERI getAllByIsEmri(int id)
        {
            return efIsEmrii.Get(id);
        }

        public int getWhereIsEmiNoToID(string IS_EMRI_NO)
        {
            try
            {
                if(efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO && x.STATU == "Tamamlandi") != null)
                {
                    return efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO && x.STATU == "Tamamlandi").IS_EMRI_ID;
                }
             
               return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public int getIsEmri(string IS_EMRI_NO)
        {
            try
            {
                if (efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO) != null)
                {
                    return efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO).IS_EMRI_ID;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public string getWhereIsEmiNoToSTATU(string IS_EMRI_NO)
        {
            try
            {
                if (efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO) != null)
                {
                    return efIsEmrii.GetFistOrDefault(x => x.IS_EMRI_NO == IS_EMRI_NO).STATU;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }


        public int Save(IS_EMIRLERI model)
        {
            return efIsEmrii.Save(model);
        }

        public int Update(IS_EMIRLERI model)
        {
            return efIsEmrii.Update(model);
        }

    }
}
