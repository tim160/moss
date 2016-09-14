
namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// Implement and register this interface to be called after a commit fails in the UOW.Dispose().
    /// Every implementation must be registered as Transient (not Singleton).
    /// </summary>
    /// <remarks>
    /// This interface is used to clean up non-transaction items which are not rolled back with a transaction rollback.
    /// I.e. an in-memory cache has been updated as work has been done during a UOW transaction and the transaction fails/rollback. 
    /// The modified in-memory cache might get out of date because the modifications don't match with the rolled back transaction.
    /// One solution is to invalidate all in-memory items which have been touched during the rolled back UOW transaction.
    /// <para>
    /// Never throw an exception - log and handle locally. 
    /// </para>
    /// </remarks>

    public interface ICommitFailureHandler
    {
        /// <summary>
        /// Get all information out of the current context to call <see cref="Process"/> later on when the context is not available anymore.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Important: every information stored for later access in <see cref="Process()"/> must be detached (DTOed). In Process()
        /// we don't have access to the UOW or any models any more.
        /// </para>
        /// <para>
        /// This is being called before Flush() or Commit() is processed or after an exception is thrown in our code base.
        /// The UOW context is still alive and can be accessed.
        /// </para>
        /// <para>
        /// Don't keep a reference to the context (get copies of all information needed to call <see cref="Process"/>) - it is 
        /// not guaranteed that the context still exists once <see cref="Process"/> is being called.
        /// </para>
        /// This should be idempotent and NOT throw any exception! If an error occurs, catch it and take according measures (i.e. invalidate
        /// the whole cache instead of just pieces.
        /// <para>
        /// This is called before any Flush() and before any Commit() or after an error in our code base was thrown.
        /// </para>
        /// </remarks>
        /// <param name="context">Current context to extract all information needed to execute this failure handler</param>

        void GetContextDataBefore(IMLSDbContext context);

        /// <summary>
        /// Clean up work if a transaction (UOW) fails.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Important: every information referencing model objects of the UOW is invalid - model objects should have
        /// been converted into DTOs.
        /// </para>
        ///  <para>
        /// This is called in the Dispose() of the UOW if the commit was not successful or an exception was thrown out of our code base.
        /// </para>
        /// This should be idempotent and NOT throw any exception! If an error occurs, catch it and take according measures (i.e. invalidate
        /// the whole cache instead of just pieces.
        /// </remarks>

        void Process();

        /// <summary>
        /// The order the handler should be executed according to other failure handlers. They will execute in ascending order (e.g. 1000 before 2000).
        /// </summary>

        int Order { get; }

    }
}
