using System.Collections.Generic;

namespace Bb.Web
{

    /// <summary>
    /// generic interface for curd service
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IServiceCrud<TKey, TEntity>
        where TKey : class
        where TEntity : class, IEntity<TKey>
    {

        /// <summary>
        /// return the list of entity
        /// </summary>
        /// <returns> list of <see cref="TEntity"/></returns>
        IEnumerable<TEntity> ReadAll();

        /// <summary>
        /// Creates or update entity specified.
        /// </summary>
        /// <param name="entity">The entity that you must to save.</param>
        /// <returns><see cref="TEntity"/></returns>
        void Save(TEntity entity);

        /// <summary>
        /// Reads the entity for specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><see cref="TEntity"/></returns>
        TEntity Read(TKey key);

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="entity">The entity.</param>
        bool Delete(TKey key);

    }


}
