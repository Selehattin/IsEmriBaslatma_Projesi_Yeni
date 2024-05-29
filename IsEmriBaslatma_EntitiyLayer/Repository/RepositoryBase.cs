using IsEmriBaslatma_DataAccesLayer.Model;

namespace IsEmriBaslatma_EntitiyLayer.Repository
{
    public class RepositoryBase
    {
        //Static
        protected IsEmriDbEntities db;
        private readonly object _lockSync = new object();
        public RepositoryBase()
        {
            db = CreateContext();
        }

        private IsEmriDbEntities CreateContext()
        {
            if (db == null)
            {
                lock (_lockSync)
                {
                    if (db == null)
                    {
                        db = new IsEmriDbEntities();
                    }
                }
            }
            return db;
        }
    }
}
