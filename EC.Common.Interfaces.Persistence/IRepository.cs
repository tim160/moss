using System;
using System.Linq;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// A Simple Repository is a container for managing a set of objects of a given type.
    /// A Simple Repository can represent either a collection of transient objects or
    /// a collection of persistent objects. The derived interface IRepository is specifically
    /// for managing a set of persistent objects. During normal operation all SimpleRepositories
    /// are actually instances of IRepository, and are in fact managing persistent EF objects.
    /// However, ISimpleRepository allows model classes to be persistent ignorant, and during
    /// unit testing, instance if ISimpleRepository will be implemented by a straightforward
    /// class that manages a set of objects in memory (that latter class has not been written 
    /// yet because we don't have any unit tests at the moment).
    /// </summary>
    /// <typeparam name="I">interface for the managed objects</typeparam>

    public interface ISimpleRepository<I>
    {
        /// <summary>
        /// Create a new persistent entity of type I.
        /// </summary>
        /// <param name="id">Optional: set specific Id for the item (by default a new Guid is generated)</param>

        I CreateItem(Guid? id = null);

        /// <summary>
        /// Delete the given entity and its dependent items 
        /// (which would become orphans otherwise) from storage.
        /// </summary>
        /// <remarks>
        /// Note that the only field of the parameter that is meaningful is ID.
        /// </remarks>
        /// <param name="n">entity to delete</param>

        void DeleteItem(I n);

        /// <summary>
        /// Return an interface-typed IQueryable for access to the DB.
        /// (this queryable does not actually retrieve anything until a query is executed).
        /// <remarks>
        /// To reduce DB round trips you can include navigation properties to preload (meaning include). 
        /// Override this method in a concrete implementation to reduce DB round trips.
        /// </remarks>
        /// </summary>
        /// <param name="propertyPathsToInclude">Optional property paths to include (bulk load). See remarks for details. Set <c>null</c> if no property preload is needed.</param>

        IQueryable<I> QueryDB(string propertyPathsToInclude = null);
        
        /// <summary>
        /// Find an object based on its primary key. This operation is directly supported by EF
        /// and is supposed to be fast.
        /// </summary>

        I FindObjectById(Guid id);
        
    }

    /// <summary>
    /// IRepository represents a collection of persistent objects. The objects represented must all conform 
    /// to interface I.
    /// </summary>
    /// <typeparam name="I">Interface to the objects in the repository</typeparam>

    public interface IRepository<I> : ISimpleRepository<I>
    {
        /// <summary>
        /// Initialize an object for the first time (i.e. that is, when it becomes
        /// persistent). This method should only be used when a transient object
        /// is about to become persistent by attaching it to an existing
        /// persistent object.
        /// </summary>
        /// <param name="n">Object to initialize</param>
        /// <param name="id">Set specific Id for the item (if <c>null</c> a new Guid is applied)</param>

        void InitializeObject(I n, Guid? id);

        /// <summary>
        /// Take an existing object (instantiated directly, not by EF) and register it
        /// with the DB. If the object already exists (e.g. an object is in the DB with
        /// the same primary key) this will overwrite the object. If the object is not
        /// already in the DB, this will create it.
        /// </summary>
        /// <param name="n">model object to add make persistent</param>
        /// <param name="isNew">true: the object does not exist in the DB currently</param>

        void MakePersistent(I n, bool isNew = false);

        /// <summary>
        /// Initialize the DB with any specific data needed by this repository. 
        /// </summary>
        /// <remarks>
        /// This is typically only needed when the repository contains "singleton"
        /// objects that are effectively part of the system configuration.
        /// </remarks>

        void InitializeDB();
    }

    /// <summary>
    /// This interface provides the ability to look up Repositories. It is used to provide
    /// model classes with the ability to find/use repositories without being exposed
    /// to the to the full capabilities of the UoW. This allows the model classes to
    /// remain persistence-ignorant.
    /// </summary>

    public interface ISimpleRepositoryFinder
    {
        /// <summary>
        /// Return a repository (DB accessor) for the given type.
        /// </summary>
        /// <typeparam name="T">Interface type for model object that repository manages</typeparam>
        /// <returns>the repository</returns>

        ISimpleRepository<T> GetSimpleRepository<T>();


        /// <summary>
        /// Create a new transient object of type T.
        /// <remarks>
        /// NOTE: In order to call CreateInstance() with type T, there are two conditions that need to
        /// be met: (1) the implementation class for T must be marked as Transient. (2) The implementation
        /// class requires a [RegisterAsFactory(typeof(IGenericFactory&lt;T&gt;"))] attribute. If the factory 
        /// registration attribute is missing, then the call to resolve the factory in the code below will
        /// throw an exception.
        /// </remarks>
        /// </summary>
        /// <typeparam name="T">type of transient object to create</typeparam>
        /// <returns>new instance of T</returns>

        T CreateTransient<T>();

        /// <summary>
        /// Return a repository (DB accessor) for the given type, with the given repository interface
        /// </summary>
        /// <typeparam name="TModel">Interface type for model object that repository manages</typeparam>
        /// <typeparam name="TModelRepo">Interface type for the repository</typeparam>
        /// <returns>the repository</returns>

        TModelRepo GetRepository<TModel, TModelRepo>() where TModelRepo : class, IRepository<TModel>;

    }

    /// <summary>
    /// This is the full repository finder interface which provides access to repositories
    /// with interface IRepository rather than ISimpleRepository. Unlike ISimpleRepository,
    /// IRepository exposed information about the underlying persistence infrastructure.
    /// </summary>
    
    public interface IRepositoryFinder : ISimpleRepositoryFinder
    {
        /// <summary>
        /// Return a repository (DB accessor) for the given type.
        /// </summary>
        /// <typeparam name="T">Interface type for model object that repository manages</typeparam>
        /// <returns>the repository</returns>

        IRepository<T> GetRepository<T>();

        /// <summary>
        /// Return a repository (DB accessor) for the given type. The type is specified as
        /// a Type value in order to allow repositories to be looked up dynamically. This
        /// should be a rare situation, so this is not the normal method for getting a
        /// repository.
        /// </summary>
        /// The repository type could be constructed from the item type, but I'm feeling lazy
        /// so the caller needs to provide both.
        /// <remarks>
        /// </remarks>
        /// <param name="repoType">the type of the repository itself</param>
        /// <param name="repoItemType">the type of the item in the repository</param>
        /// <returns>the repository (as object, will need to be cast)</returns>

        object GetRepository(Type repoType, Type repoItemType);
    }
}
