using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EC.Common.Interfaces.Persistence;

namespace EC.Common.Interfaces.Persistence
{
    
    /// <summary>
    /// This interface can be used for a implementations for work which has to be done
    /// on Flush() and/or Commit() calls. The two flags <c>ProcessOnFlush</c> and 
    /// <c>ProcessOnCommit</c> decides when the handler is processed.
    /// <para>
    /// Register the implementation with [RegisterAsType(typeof(ICommitHandler))]
    /// and it gets registered and resolved and processed for every unit of work.
    /// </para>
    /// <para>
    /// NOTE: Flush() might be executed multiple times per transaction. Therefore a CommitHandler
    /// marked as <c>ProcessOnFlush</c> must be prepared to be called multiple times per UoW.
    /// </para>
    /// <para>
    /// NOTE: Since CommitHandlers are called very frequently, they need to be as efficient as 
    /// possible. For example, many commit handlers only have meaningful work to do when 
    /// persistent objects have been changed, so they should check the UoW type and if it
    /// is read-only, they should return immediately.
    /// </para>
    /// <para>
    /// The EF change tracker 'resets' after a Flush() has been called.  
    /// For example, if a NavPage has been added it will show up in 'added' state until you call Flush(). After calling Flush() the NavPage 
    /// doesn't show up as 'added' anymore. The item will be in 'unchanged' state.
    /// </para>
    /// </summary>
    
    public interface ICommitHandler
    {
        /// <summary>
        /// This will be called according to the properties <c>ProcessOnFlush</c> and <c>ProcessOnCommit</c> flags
        /// on either Flush() and/or Commit().
        /// </summary>
        /// <param name="context">Current context to work with</param>
        
        void Process(IMLSDbContext context);

        /// <summary>
        /// This flag decides whether the commit handler is process on all Flush() calls.
        /// Set <c>true</c> if the commit handler should be processed on Flush().
        /// Note: Keep in mind that Flush() might be called multiple times before a Commit() 
        /// is executed. Therefore, the commit handler should be able be processed multiple times per transaction.
        /// </summary>
        /// <remarks>
        /// Keep in mind that a Flush() changes the EF change tracker items. 
        /// For example, added items will be returned as unchanged after a Flush().
        /// See interface description above for more details.
        /// </remarks>
        
        bool ProcessOnFlush { get; }
        
        /// <summary>
        /// This flag decides whether the commit handler is process on Commit() only.
        /// Set <c>true</c> if the commit handler should be processed on Commit().
        /// </summary>
        
        bool ProcessOnCommit { get; }

        /// <summary>
        /// This flag decides whether the commit handler is able to process
        /// during an import (content, audits, registrations,...).
        /// </summary>
        /// <remarks>
        /// Some commit handlers create/update items according to new/added/modified items
        /// in the EF change tracker which could mess up mess up the
        /// import process. The import of items happens in phases and some commit handlers might 
        /// run in between (if Flush() is called) and create new items which are later imported.
        /// Those new or updated items might interfere with the import.
        /// Therefore some commit handlers are not compatible with the import and must not run.
        /// NOTE: Carefully check whether the commit handler is able to run during an import.
        /// </remarks>

        bool ProcessOnImport { get; }

        /// <summary>
        /// The order the handler should execute in. They will execute in ascending order (e.g. 1000 before 2000).
        /// </summary>

        int Order { get; }
    }
}
