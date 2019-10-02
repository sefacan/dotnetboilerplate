using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotnetBoilerplate.Data.Core
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly DataContextBase _context;

        private DbSet<TEntity> _entities;

        #endregion

        #region Ctor

        public Repository(DataContextBase context)
        {
            _context = context;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Error message</returns>
        protected void ThrowInvalidOperationException(DbUpdateException exception)
        {
            //rollback entity changes
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch { }
                });
            }

            try
            {
                _context.SaveChanges();
                throw new InvalidOperationException(exception.ToString());
            }
            catch (Exception ex)
            {
                //if after the rollback of changes the context is still not saving,
                //return the full text of the exception that occurred when saving
                throw new InvalidOperationException(ex.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> Find(int id)
        {
            return await Entities.FindAsync(id);
        }

        /// <summary>
        /// Get entity by linq expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entities.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get entity by linq expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public async Task Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public async Task Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.AddRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public async Task Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public async Task Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                _context.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public async Task Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public async Task Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                ThrowInvalidOperationException(exception);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();

                return _entities;
            }
        }

        #endregion
    }
}