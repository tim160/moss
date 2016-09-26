using System;
using EC.Common.Base;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Wrapper around .NET mutual exclusion locks that has some additional useful
    /// features.
    /// </summary>
    
    public interface ILock
    {
        /// <summary>
        /// Acquire the lock. Blocks until the lock is available.
        /// </summary>
        
        void Acquire();

        /// <summary>
        /// Release the lock.
        /// </summary>

        void Release();
        LockState CurrentState { get; }
        int HoldingThreadID { get; }
        string HoldingThreadName { get; }
        void SetUnheldLocks(ILock[] locks);
        long NumAcquires { get; }
        long TotalAcquireTicks { get; }
    }

    public enum LockState
    {
        Locked,
        Unlocked
    }

    public class OrderException : Exception
    {
    } ;
}
