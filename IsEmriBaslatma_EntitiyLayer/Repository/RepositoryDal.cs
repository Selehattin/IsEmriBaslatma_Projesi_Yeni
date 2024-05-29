using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace IsEmriBaslatma_EntitiyLayer.Repository
{
    public class RepositoryDal<T> : RepositoryBase, IRepository<T> where T : class
    {
        public DbSet<T> _dbSet;
        public RepositoryDal()
        {
            _dbSet = db.Set<T>();
        }
        public T Get(int id)
        {
            return _dbSet.Find(id);
        }
        public T Get(string id)
        {
            return _dbSet.Find(id);
        }
        public T GetFistOrDefault(Expression<Func<T, bool>> expression)
        {
            try
            {
                return _dbSet.FirstOrDefault(expression);
            }
            catch (Exception)
            {

                return null;
            }
        }
        public List<T> GetWhere(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).ToList();
        }
        public List<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public int DBSave()
        {
            return db.SaveChanges();
        }
        public int Save(T model)
        {
            _dbSet.Add(model);
            return db.SaveChanges();

        }
        public int Update(T model, int id)
        {

            return DBSave();
        }
        public int Update(T model)
        {
            _dbSet.AddOrUpdate(model);
            return DBSave();
        }
        public bool Update<A>(A model, int id) where A : class
        {
            throw new System.NotImplementedException();
        }
        public int Remove(T model)
        {
            _ = _dbSet.Remove(model);
            return DBSave();
        }

    }
}
