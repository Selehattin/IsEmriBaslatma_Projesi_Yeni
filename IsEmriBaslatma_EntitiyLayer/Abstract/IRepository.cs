using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IsEmriBaslatma_EntitiyLayer
{
    internal interface IRepository<T>
    {
        List<T> GetAll();
        T Get(int id);
        T GetFistOrDefault(Expression<Func<T, bool>> expression);
        int Update(T model);
        int Update(T model, int id);
        bool Update<A>(A model, int id) where A : class;
        int DBSave();
        int Save(T model);

    }
}
