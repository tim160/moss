using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// A wrapper class around Entity Framework's DbContext class. Using an
    /// </summary>

    public interface IMLSDbContext : IRepositoryFinder
    {
        /// <summary>
        /// Flushing all pending changes for this context out to the DB.
        /// </summary>
        /// <param name="isCommit">Flag whether the flush call is just before the commit (<c>true</c>). <c>False</c> otherwise</param>
        /// <param name="isImport">Indicates whether this flush is from a UoW in import mode</param>
        /// <param name="commitFailureHandlers">Commit failure handler to execure <paramref name="failureHandler"/> with</param>
        /// <param name="failureHandler">Function to call with <paramref name="commitFailureHandlers"/> and this DB context to exctract information for the failure handlers</param>

        void Flush(bool isCommit, bool isImport, IList<ICommitFailureHandler> commitFailureHandlers, Action<IList<ICommitFailureHandler>, IMLSDbContext> failureHandler);

        /// <summary>
        /// Ask the DbContext to return a mapped collection for a specific type. The type must be
        /// one that has been configured into EF via the model builder.
        /// </summary>
        /// <typeparam name="T">class for model object</typeparam>
        /// <returns>mapped collection (as DbSet<T>)</returns>

        DbSet<T> MapDbSet<T>() where T : class;
        

        /// <summary>
        /// Take an object that was instantiated directly (rather than through EF) and
        /// make it persistent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>

        void MakePersistent<T>(T obj, bool isNew = false) where T : class;

        /// <summary>
        /// Marks an object as detached.
        /// </summary>

        void MakeTransient<T>(T obj) where T : class;

        /// <summary>
        /// Manually cascades the deleting of an object.
        /// This means that all dependent items will be deleted too.
        /// To get the dependent items the implementation of the item must 
        /// implement the interface <c>ICascadeDelete</c>.
        /// </summary>
        /// <param name="item">The item to delete</param>

        void CascadeDelete(object item);

        /// <summary>
        /// Find an object in the current DB context that satisfies the given predicate. This
        /// method is useful if you need to find an object, but that object may have already
        /// been modified or added within the current unit of work. Just using the
        /// repository to find such objects can fail because the queries are made against
        /// the DB and the results don't reflect any changes made within the transaction.
        /// </summary>
        /// <typeparam name="T">the concrete type of the object </typeparam>
        /// <typeparam name="I">the interface type of the object </typeparam>
        /// <param name="predicate">predicate the object must satisfy</param>
        /// <returns>the object or null</returns>

        T FindObject<T, I>(Expression<Func<T, bool>> predicate) where T : class, I;

        /// <summary>
        /// Find an objects in the current DB context that satisfies the given predicate. This
        /// method is useful if you need to find all objects, but objects may have already
        /// been modified or added within the current unit of work. Just using the
        /// repository to find such objects can fail because the queries are made against
        /// the DB and the results don't reflect any changes made within the transaction.
        /// </summary>
        /// <typeparam name="T">the concrete type of the object </typeparam>
        /// <typeparam name="I">the interface type of the object </typeparam>
        /// <param name="predicate">predicate the object must satisfy</param>
        /// <returns>the objects or null</returns>

        IEnumerable<T> FindObjects<T, I>(Expression<Func<T, bool>> predicate) where T : class, I;

        /// <summary>
        /// Find objects with Ids that match
        /// </summary>
        /// <remarks>
        /// We have created this specialized method to take advantage of chunking
        /// </remarks>
        IEnumerable<T> FindObjectsByIds<T, I>(IEnumerable<Guid> ids) where T : class, I, IHasGuidId;

        /// <summary>
        /// Count objects according to <paramref name="predicate"/>.
        /// No Flush() needed to access modified/added elements.
        /// </summary>
        /// <remarks>This works well with a <paramref name="predicate"/> that really narrows down the scope for the db request. 
        /// And if the amount of cached objects is small. Otherwise calling except on the sql query might not be performant.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="predicate">Predicate to query the elements</param>
        /// <returns></returns>

        int CountObjects<T, I>(Expression<Func<T, bool>> predicate) where T : class, I;

        /// <summary>
        /// Return the set of entities that have been added, deleted, or modified in this context.
        /// </summary>
        /// <remarks>
        /// It would be better if this method took an interface for the entity type rather than
        /// a concrete model. That would require the MLSDBContext to maintain a mapping of
        /// interfaces to concrete types. That would be possible, but is being left as an
        /// exercise for future refactoring.
        /// </remarks>
        /// <typeparam name="T">concrete class for model type</typeparam>
        /// <returns>list of changed entities</returns>

        IEnumerable<T> GetChangedEntities<T>() where T : class;

        /// <summary>
        /// Returns the set of entities that are in the <paramref name="state"/> EntityState.
        /// </summary>
        /// <remarks>
        /// It would be better if this method took an interface for the entity type rather than
        /// a concrete model. That would require the MLSDBContext to maintain a mapping of
        /// interfaces to concrete types. That would be possible, but is being left as an
        /// exercise for future refactoring.
        /// </remarks>
        /// <typeparam name="T">concrete class for model type</typeparam>
        /// <param name="state">State of entities returned</param>
        /// <returns>list of changed entities with an EntityState = <paramref name="state"></paramref>/></returns>

        IEnumerable<T> GetChangedEntitiesByState<T>(EntityState state) where T : class;

        /// <summary>
        /// Returns the set of DB entities for T that are in the <paramref name="state"/> EntityState.
        /// </summary>
        /// <remarks>
        /// It would be better if this method took an interface for the entity type rather than
        /// a concrete model. That would require the MLSDBContext to maintain a mapping of
        /// interfaces to concrete types. That would be possible, but is being left as an
        /// exercise for future refactoring.
        /// <para>
        /// If <paramref name="state"/> = Modified | Added
        /// all items which are in state Modified OR Added are returned.
        /// </para>
        /// <para>
        /// Access model object of type T via <c>Entity</c>.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">concrete class for model type</typeparam>
        /// <param name="state">State of entities returned (multiple states can be flagged together)</param>
        /// <returns>list of DB entities with an EntityState = <paramref name="state"/></returns>

        IEnumerable<DbEntityEntry<T>> GetChangedDbEntitiesByState<T>(EntityState state) where T : class;

        /// <summary>
        /// Return the set of entities (independent of the type) that are in the <paramref name="state"/> EntityState.
        /// </summary>
        /// <param name="state">State of entities returned</param>
        /// <returns>List of changed entities with an EntityState = <paramref name="state"/></returns>

        IEnumerable<object> GetChangedEntitiesByState(EntityState state);

        /// <summary>
        /// Return the set of entities (of type T) from this context that are modified, but ignoring changes 
        /// in any of the given properties. For example, making this call with T = IUser and excluding 
        /// 'LastLogin' will return user objects which have been modified, unless the only property that
        /// was modified the 'LastLogin' property.
        /// </summary>

        IEnumerable<T> GetChangedEntitiesExcludingProperties<T>(IEnumerable<string> propertiesToExclude) where T : class;

        /// <summary>
        /// Count all tracked entries (created, deleted, added or modified) in the ChangeTracker.
        /// </summary>
        /// <returns>Count of all tracked entries</returns>

        int GetTrackedItemsCount();

        /// <summary>
        /// Return the set of entities that have been deleted in this context.
        /// </summary>
        /// <typeparam name="T">concrete class for model type</typeparam>
        /// <returns>Return all deleted entities of type <c>T</c></returns>

        IEnumerable<T> GetDeletedEntities<T>() where T : class;

        /// <summary>
        /// Returns the set of entities which are the principal of a relationship which has been
        /// added/removed in this context. There is some redundancy in the parameters in that
        /// you have to specify the string name for the concrete type, as well as using that type as 
        /// type parameter. This is due to an apparent limitation in EF; they don't appear to
        /// store the type of the objects in the relationship, just the name of the table that
        /// the objects are stored in.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In EF, an entity that has had a navigational property altered (e.g. an object added to
        /// one of its ICollection navigation properties) is not marked as altered itself. Instead
        /// you have to look for changed relationships, and then find the objects on the ends of
        /// those relationships. This calls only looks at the "principal" (as opposed to "dependent")
        /// side of each relationship.
        /// </para>
        /// As with GetChangedEntities, it would be better if this method could work in terms of
        /// model interfaces rather than concrete object types.
        /// <para>
        /// </para>
        /// </remarks>
        /// <typeparam name="T">concrete model type for entity.</typeparam>
        /// <param name="tableName">the DB table containing the principal entities</param>
        /// <returns>list of principals with changed relationships</returns>

        IEnumerable<T> GetEntitiesWithChangedRelationships<T>(string tableName) where T : class;


        /// <summary>
        /// Gets the changed relationship target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relationshipName">Name of the relationship.</param>
        /// <returns></returns>
        IEnumerable<T> GetAddedTargetsForRelationship<T>(string relationshipName) where T : class;

        /// <summary>
        /// Gets the added targets for relationship.
        /// </summary>
        /// <param name="relationshipName">Name of the relationship.</param>
        /// <returns></returns>
        IEnumerable<PairedTypeItem<T1, T2>> GetAddedTargetsForRelationship<T1, T2>(string relationshipName)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Gets the modified targets for relationship.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="relationshipName">Name of the relationship.</param>
        /// <returns></returns>
        IEnumerable<PairedTypeItem<T1, T2>> GetModifiedTargetsForRelationship<T1, T2>(string relationshipName)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Gets the removed target for relationship.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relationshipName">Name of the relationship.</param>
        /// <returns></returns>
        IEnumerable<T> GetRemovedTargetsForRelationship<T>(string relationshipName) where T : class;

        /// <summary>
        /// Gets the removed targets for relationship.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="relationshipName">Name of the relationship.</param>
        /// <returns></returns>
        IEnumerable<PairedTypeItem<T1, T2>> GetRemovedTargetsForRelationship<T1, T2>(string relationshipName)
            where T1 : class
            where T2 : class;
        /// <summary>
        /// Check whether the actual state of modified entities is consistent with the type of
        /// transaction being executed. E.g. a read-only transaction should not have any 
        /// entities in a modified state.
        /// </summary>
        /// <remarks>
        /// NOTE: This code only runs in DEBUG builds.
        /// </remarks>
        /// <param name="UoWType">the type of transaction being committed.</param>

        void CheckTransactionConsistency(UnitOfWorkTypes UoWType);

        /// <summary>
        /// Set the Unit Of Work which holds this context. Except for EF migration mode
        /// contexts, all MLSDBContext instances are held by a corresponding UnitOfWork. That is,
        /// a UnitofWork has a required MLSDBContext, and an MLSDBContext has an optional
        /// owning UnitofWork.
        /// </summary>
        /// <param name="uow">the unit of work which holds this context.</param>
        
        void SetOwner(IUnitofWork uow);

        /// <summary>
        /// Return the unit of work that owns this context.
        /// </summary>
        /// <returns>the Unit of Work</returns>
        
        IUnitofWork GetOwner();

        /// <summary>
        /// Connection name for the DB connection string.
        /// </summary>

        string ConnectionString { get; }

        /// <summary>
        /// Returns the real connection string in DEBUG builds, and a redacted one in release builds.
        /// </summary>
        string ConnectionStringForLogging { get; }

        /// <summary>
        /// This checks integrity of the DB and execute possible migrations.
        /// <para>
        /// The actual implementation of this is deferred to each concrete DB context
        /// class. However, the typical implementation will attempt to load one EF
        /// entity from any table. This will both force EF to invoke any migrations necessary,
        /// as well as verifying that it is possible to access the DB.
        /// </para>
        /// Every concrete IUnitofWork implementation needs to register its specific 
        /// DB context as shown below in order for the generic startup code to be
        /// able to locate all DB context instances and validate them.
        /// <para>
        /// e.g.  [RegisterAsType(typeof(ICSmartDBContext), typeof(IMLSDBContext))]
        /// </para>
        /// <example> 
        /// public override void ValidateDatabase()
        /// {
        ///    var basicRepo = this.GetRepository<IBasicItem>();
        ///    basicRepo.All().FirstOrDefault();
        /// }
        /// </example>
        /// </summary>

        void ValidateDatabase();

        /// <summary>
        /// Used to execute a SQL query on the DB.  
        /// </summary>
        /// <typeparam name="T">Concreat type of object to be queried</typeparam>
        /// <param name="sql">SQL string to execute on Db</param>
        /// <returns>IQueryable<T></returns>

        IQueryable<T> SQL<T>(string sql);

        /// <summary>
        /// Used so execute an SQL on the DB.  
        /// </summary>
        /// <param name="sql">SQL string to execute on Db</param>

        void SQL(string sql);

        /// <summary>
        /// Flag set to <c>false</c> if flush/savechanges errors on Flush().
        /// That means the context can't be accessed anymore.
        /// </summary>

        bool IsContextValid { get; set; }

        /// <summary>
        /// Set true when commit handler times should be logged
        /// </summary>
        bool LogCommitTimes { get; set; }

        int GuidChunkSize { get; }
    }
    //these are use to represent pair of object involve in entity relationship, use in GetRemovedTargetsForRelationship,GetAddedTargetsForRelationship method
    public class PairedTypeItem<T1, T2>
    {
        public T1 LHS { get; set; }
        public T2 RHS { get; set; }
    }
    public class PairedObject
    {
        public object LHS { get; set; }
        public object RHS { get; set; }
    }
    public class PairedEntityKey
    {
        public EntityKey LHSKey { get; set; }
        public EntityKey RHSKey { get; set; }
    }
}
