using System;

namespace Bb.Web
{

    /// <summary>
    /// Generic base for entity object
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IEntity<TKey>
        where TKey : class
    {


        TKey Identifier { get; set; }

        /// <summary>
        /// Gets or sets the update.
        /// </summary>
        /// <value>
        /// The update.
        /// </value>
        DateTimeOffset Updated { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        DateTimeOffset Created { get; set; }

    }


}
