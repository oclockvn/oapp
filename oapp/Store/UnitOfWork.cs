using oapp.Entities;
using oapp.Extensions;
using oapp.Helpers;
using oapp.Repos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace oapp.Store
{
    public interface IUnitOfWork : IDisposable
    {
        Tuple<bool, string> SaveChanges();
        TRepository Get<TRepository>() where TRepository : class, IRepository;
        IRepository<T> GetByType<T>() where T : TEntity;
    }

    public class UnitOfWork : IUnitOfWork
    {
        private static readonly NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
        private readonly Dictionary<Type, Lazy<IRepository>> repos;
        private Lazy<DbContext> db;
        private bool disposed;

        public UnitOfWork(DbContext db)
        {
            this.db = new Lazy<DbContext>(() => db);
            repos = new Dictionary<Type, Lazy<IRepository>>
            {
                // todo: regiter repositories here
                // ex: { typeof (IOrderRepository), new Lazy<IRepository>(() => new OrderRepository(db.Value)) },
            };

#if DEBUG
            this.db.Value.Database.Log = msg => System.Diagnostics.Debug.WriteLine(msg);
#endif
        }

        public Tuple<bool, string> SaveChanges()
        {
            var msg = string.Empty;

            try
            {
                db.Value.SaveChanges();
                return new Tuple<bool, string>(true, msg);
            }
            catch(DbEntityValidationException ex)
            {
                msg = string.Join(", ", 
                    ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .SelectMany(e => $"Property: {e.PropertyName} -> {e.ErrorMessage}"));                    
            }
            catch(Exception ex)
            {
                msg = ex.ToErrorMessage();
            }

            // send email to config account
            MailHelper.SendMail(string.Empty, string.Empty, "Save changes error", msg);

            return new Tuple<bool, string>(false, msg);
        }

        public TRepository Get<TRepository>() where TRepository : class, IRepository
        {
            if (!repos.ContainsKey(typeof(TRepository)))
                throw new Exception($"Repository of type {typeof(TRepository)} have not been registered!");
            
            return repos[typeof(TRepository)].Value as TRepository;
        }

        public IRepository<T> GetByType<T>() where T : TEntity
        {
            return new Repository<T>(db.Value);
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
                    db.Value?.Dispose();
                }

                disposed = true;
            }
        }
    }
}