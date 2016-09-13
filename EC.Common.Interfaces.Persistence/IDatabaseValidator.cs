using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.MicroKernel;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// Validates/checks all UoWs registered with the kernel
    /// and tries to connect to the DBs and also checks its
    /// integrities.
    /// </summary>

    public interface IDatabaseValidator
    {
        /// <summary>
        /// Check to see whether the application can connect to all extension DBs and whether their integrity
        /// is ok.
        /// Shutdown if one of the DBs are not in a valid state.
        /// </summary>
        /// <remarks>
        /// This code is needed because EF takes a long time to time out if you simply try to access
        /// an entity but the underlying DB is not there. This allows us to fail very quickly if the
        /// DBs are not there.
        /// </remarks>
        /// <exception cref="NotFoundException">If one of the DBs are not accessible or don't exist.</exception>

        void CheckDatabases();
    }
}
