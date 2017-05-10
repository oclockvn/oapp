using oapp.Entities;
using oapp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace oapp.Repos
{
    public interface IRepository : IDisposable
    {}

    public interface IRepository<T> : IRepository where T : TEntity
    {
        /// <summary>
        /// return IQueryable object from dataset
        /// </summary>
        IQueryable<T> All { get; }

        /// <summary>
        /// get list of T entity with given condition and order.
        /// If includes has values, load related properties with eager loading
        /// </summary>
        /// <param name="where">the predicate to filter</param>
        /// <param name="orderBy">order result by order condition</param>
        /// <param name="includes">includes related properties to load</param>
        /// <returns>list of T entity</returns>
        List<T> List(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Find the very first of entity with given condition.
        /// This method is same as List, but take top 1 element
        /// </summary>
        /// <param name="where">the predicate to filter</param>
        /// <param name="includes">includes related properties to load</param>
        /// <returns>First or default entity</returns>
        T Find(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Find element by primary key
        /// </summary>
        /// <param name="id">the primary key of entity, and it must be number</param>
        /// <returns>Found element or null</returns>
        T GetById(object id);

        /// <summary>
        /// add entity to table
        /// </summary>
        /// <param name="model">the entity model to add</param>
        /// <returns>added entity model</returns>
        T Create(T model);

        /// <summary>
        /// update given entity with or without specific properties (if provide)
        /// </summary>
        /// <param name="model">entity model to update</param>
        /// <param name="updateProperties">if provide any properties, update these properties instead of all entity</param>
        void Update(T model, List<Expression<Func<T, object>>> updateProperties = null);
        
        /// <summary>
        /// delete entity by id.
        /// </summary>
        /// <param name="id">primary key of id to delete</param>
        /// <returns>true if deleted entity is not null, otherwise return false</returns>
        int Delete(int id);

        /// <summary>
        /// delete set of entities by condition
        /// </summary>
        /// <param name="where">The condition to filter entities</param>
        /// <returns>Total number of deleted entities</returns>
        int Delete(Expression<Func<T, bool>> where);
    }

    public class Repository<T> : IRepository<T> where T : TEntity
    {
        private DbContext db;
        private readonly DbSet<T> table;
        protected bool disposed;

        public Repository(DbContext context)
        {
            db = context;
            table = context.Set<T>();
        }

        public IQueryable<T> All => table.AsQueryable();
        public T GetById(object id) => table.Find(id);
        public T Create(T model) => table.Add(model);

        public int Delete(int id) => Delete(x => x.Id == id);
        
        public int Delete(Expression<Func<T, bool>> where)
        {
            var entities = All.Where(where);
            foreach (var t in entities)
            {
                table.Remove(t);
            }

            return table.Count();
        }

        public T Find(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var all = All;

            if (includes != null && includes.Length > 0)
            {
                all = all.Including(includes);
            }

            T result = default(T);

            if (where != null)
            {
                result = all.FirstOrDefault(where);
            }

            return result;
        }
        
        public List<T> List(Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            var all = All;

            if (includes != null && includes.Length > 0)
            {
                all = all.Including(includes);
            }

            if (where != null)
            {
                all = all.Where(where);
            }

            orderBy?.Invoke(all);

            return all.AsNoTracking().ToList();
        }

        public void Update(T model, List<Expression<Func<T, object>>> updateProperties = null)
        {
            table.Attach(model);

            if (updateProperties != null && updateProperties.Count > 0)
            {
                updateProperties.ForEach(prop =>
                {
                    db.Entry(model).Property(prop).IsModified = true;
                });
            }
            else
            {
                db.Entry(model).State = EntityState.Modified;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                        db = null;
                    }
                }

                disposed = true;
            }
        }

        #region direct query
        private SqlParameter Param(string key, object value)
        {
            var s = value as string;
            if (s != null && string.IsNullOrEmpty(s))
                value = string.Empty;

            return new SqlParameter(key, value);
        }

        protected int ExecuteSqlCommand(string command, params object[] args) => db.Database.ExecuteSqlCommand(command, args);
        protected List<TResult> SqlQuery<TResult>(string query, params object[] args) => db.Database.SqlQuery<TResult>(query, args).ToList();
        #endregion
    }
}