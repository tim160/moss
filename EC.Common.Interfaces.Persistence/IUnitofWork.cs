using System;
using System.Collections.Generic;
using System.Text;
using EC.Constants;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// Interface for object used to wrap access to persistent objects. The intent
    /// is that an instance of IUnitOfWork is used within a "using" block. Since
    /// MS recommends that DBContext's be short-lived, they are wrapped inside the
    /// unit of work, and the intent is that each unit of work defines a transaction
    /// boundary as well.
    /// <para>
    /// NOTE: As of May 1, 2014 all code based on MarineLMS.Persistence has been
    /// switched to use snapshot isolation (i.e. optimistic concurrency control).
    /// In order to provide automatic transaction retry in the case of update
    /// conflicts, the new helper class IUOW/UOW has been created. This class
    /// should be used to wrap blocks of code in transactions.
    /// </para>
    /// </summary>

    public interface IUnitofWork : IDisposable, IRepositoryFinder
    {
        /// <summary>
        /// Commit this UnitofWork to the DB.
        /// </summary>

        void Commit();

        /// <summary>
        /// Signal that the caller is going to make write modifications to persistent objects
        /// that are going to be manually reversed. If the transaction is ReadOnly, this
        /// will change it to WriteAbandon. This will cause the transaction to throw away
        /// any writes done within the transaction. If the transaction is Write, then the
        /// type remains Write, and any persistent modifications will be written out on
        /// Commit.
        /// </summary>

        void StartAbandonedWrites();

        /// <summary>
        /// Signal that the caller is no longer making write modifications that will be
        /// manually reversed.
        /// </summary>

        void FinishAbandonedWrites();

        /// <summary>
        /// Returns true if the UoW is currently in the state where abandoned writes are
        /// being made.
        /// </summary>
        /// <returns>current abandon write state</returns>

        bool AbandonedWriteModeActive();

        /// <summary>
        /// Get the DB context that is bound to this unit of work.
        /// </summary>
        /// <returns>the DB context</returns>

        IMLSDbContext GetDBContext();

        UnitOfWorkTypes UoWType { get; }
        
        /// <summary>
        /// Flag whether the UoW is in import mode and therefore omits some commit handlers.
        /// </summary>

        bool IsImport { get; }

        /// <summary>
        /// Flag that describes the import mode, if the UoW is in import mode.
        /// </summary>

        ImportModesEnum? ImportMode { get; }
    }

    /// <summary>
    /// Every unit of work must indicate how it will access data. There are 3 modes:
    /// <para>
    /// ReadOnly - the UoW will only read persistent data
    /// </para>
    /// <para>
    /// Write - the UoW will both read and write persistent data
    /// </para>
    /// <para>
    /// WriteAbandon - the UoW will write persistent data, but will always abort the
    /// transaction. This is only used in special "what if" circumstances in which 
    /// one is modifying persistent data to validate certain conditions.
    /// </para>
    /// <para>
    /// WriteMaybe - the UoW may or may not write data. This should be used rarely. It is
    /// for situations in which the decision to make persistent changes is actually computed
    /// in the body of the transaction. One example of this is the background transaction for
    /// purging old entries from the session table; it only makes changes if there are
    /// sessions which have timed out and this is determined within the body of the transaction.
    /// This mode is treated as "Write" except that it suppresses logging about the
    /// transaction not making any changes.
    /// </para>
    /// </summary>

    public enum UnitOfWorkTypes
    {
        ReadOnly,
        WriteAbandon,
        Write,
        WriteMaybe
    }
}
