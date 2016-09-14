using System;

namespace EC.Common.Interfaces.Persistence
{

    /// <summary>
    /// This class provides a block-structure like mechanism for excuting code in a unit of work.
    /// This provides functionality that is similar to a 'using' statement, but allows for more
    /// complex error handling, such as automatically re-trying a unit of work which has failed
    /// due to a deadlock.
    /// </summary>
    
    public interface IUOW
    {
        /// <summary>
        /// Execute a block of code within a read committed snapshot unit of work, with retries.
        /// </summary>
        /// <remarks>
        /// Retry <paramref name="code"/> up to 7 times on deadlock and DbUpdateException in an isolated transaction.
        /// Wait in between retries. Worst case all 7 times fail and total time spent is 6sec.
        /// </remarks>
        /// <typeparam name="T">which type of unit of work</typeparam>
        /// <param name="type">transaction type (read-only, etc)</param>
        /// <param name="code">code to execute</param>
        /// <exception cref="UowIsolationException">If retries failed to update the <c>code</c>/deadlock</exception>

        void Do<T>(UnitOfWorkTypes type, Action<T> code) where T : IUnitofWork;

        /// <summary>
        /// Execute a block of code within a read committed snapshot unit of work, with retries. 
        /// This variant is for use when the block of code needs to return a value.
        /// </summary>
        /// <remarks>
        /// Retry <paramref name="code"/> up to 7 times on deadlock and DbUpdateException in an isolated transaction.
        /// Wait in between retries. Worst case all 7 times fail and total time spent is 6sec.
        /// </remarks>
        /// <typeparam name="T">which type of unit of work</typeparam>
        /// <param name="type">transaction type (read-only, etc)</param>
        /// <param name="code">code to execute</param>
        /// <exception cref="UowIsolationException">If retries failed to update the <c>code</c>/deadlock</exception>

        R Do<T, R>(UnitOfWorkTypes type, Func<T, R> code) where T : IUnitofWork;

        /// <summary>
        /// Execute a block of code within a fully isolated unit of work, with retries.
        /// </summary>
        /// <remarks>
        /// Retry <paramref name="code"/> up to 7 times on deadlock and DbUpdateException in an isolated transaction.
        /// Wait in between retries. Worst case all 7 times fail and total time spent is 6sec.
        /// </remarks>
        /// <typeparam name="T">which type of unit of work</typeparam>
        /// <param name="type">transaction type (read-only, etc)</param>
        /// <param name="code">code to execute</param>
        /// <exception cref="UowIsolationException">If retries failed to update the <c>code</c>/deadlock</exception>

        void Isolated<T>(UnitOfWorkTypes type, Action<T> code) where T : IUnitofWork;

        /// <summary>
        /// Execute a block of code within am isolated unit of work, with retries. 
        /// This variant is for use when the block of code needs to return a value.
        /// </summary>
        /// <remarks>
        /// Retry <paramref name="code"/> up to 7 times on deadlock and DbUpdateException.
        /// Wait in between retries. Worst case all 7 times fail and total time spent is 6sec.
        /// </remarks>
        /// <typeparam name="T">which type of unit of work</typeparam>
        /// <param name="type">transaction type (read-only, etc)</param>
        /// <param name="code">code to execute</param>
        /// <exception cref="UowIsolationException">If retries failed to update the <c>code</c>/deadlock</exception>

        R Isolated<T, R>(UnitOfWorkTypes type, Func<T, R> code) where T : IUnitofWork;
    }
}
