using System;
using System.Reflection;
using EC.Common.Base;
using EC.Common.Interfaces;
using EC.Errors;

namespace EC.Core.Common
{
    /// <summary>
    /// This class provides the scoped lock feature by implementing
    /// the IDisposable interface. In general, only objects that hold
    /// on to scare resource should implement the IDisposable interface.
    /// We are using this trick to help implementing the scoped lock
    /// feature because there is no real destructor in C#.
    /// <para>
    /// Construct the ScopedLock within the "using" statement will cause
    /// the underlying lock to release when the "using" block exits.
    /// <example>
    /// <code>
    /// using (ScopedLock myScopedLock = new ScopedLock(mLock)
    /// {
    ///     some code...
    /// }  &lt;-- mLock will be release automatically when the "using"
    ///        block exits
    /// </code>
    /// </example>
    /// </para>
    /// <remarks>
    /// Even though this class resides at the Core.Common level it
    /// is not set up to work through IoC because it works most
    /// conveniently with a constructor parameter -- which is not quite
    /// as pleasant when you are using IoC.
    /// </remarks>
    /// </summary>
   
    public sealed class ScopedLock : IDisposable
    {
        /// <summary>
        /// Release the underlying Lock that was passed in through the
        /// constructor.
        /// </summary>
        /// <remarks>
        /// Calling Dispose multiple times is not harmful (but not recommended).
        /// <para>
        /// GC.SuppressFinalize is called at the end of a successful Dispose
        /// to help claiming the memory back quicker.
        /// </para>
        /// </remarks>
        
        public void Dispose()
        {
            DBC.NonNull(_lockObj, "Underlying Lock is null");

            try
            {
                // shouldn't need to lock because ScopedLock should be local to methods
                // but just in case it's not being used properly
                lock (this)
                {
                    if (!_disposed && _lockObj != null)
                    {
                        _lockObj.Release();

                        // want to make sure ScopeLock will only release the lock once
                        // even multiple Dispose was called.
                        _disposed = true;

                        // it's already disposed, notify the GC not to call our
                        // finalize method
                        GC.SuppressFinalize(this);
                    }
                }
            }
            catch (Exception ex)
            {
                DBC.Assert(false, "Exception thrown in ScopedLock::Dispose(), ex=" + ex.ToString());
            }
        }

        /// <summary>
        /// Constructor. Initialize the internal Lock object from the
        /// <paramref name="lockObj"/> parameter.
        /// </summary>
        /// <param name="lockObj">The underlying Lock that ScopedLock
        /// is going to operate on.</param>

        public ScopedLock(ILock lockObj)
        {
            DBC.NonNull(lockObj, "Incoming param is null");

            if (lockObj != null)
            {
                _lockObj = lockObj;
                _lockObj.Acquire();
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        /// <remarks>
        /// Strictly speaking there is no need to define the finalizer, it's defined
        /// to help debugging user errors in using the ScopedLock (e.g. forget
        /// to place the ScopeLock inside the "using" statement).
        /// <para>It just logs the error and didn't try to release the underlying
        /// lock at all (in fact, we can't because the current executing thread is 
        /// the finalizer thread.
        /// </para>
        /// </remarks>
        
        ~ScopedLock()
        {
            WindowsEventLog.AddEvent("~ScopedLock: The Dispose method hasn't been called.", WindowsEventLog.LockingError);
        }

        // -------------------------------------- Instance Data -----------------------------------

        private readonly ILock _lockObj;      // the underlying Lock object
        private bool _disposed;               // disposed already?

    }
}