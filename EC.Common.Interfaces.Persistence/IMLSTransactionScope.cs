using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// Wrapper around TransactionScope class so that we can do unit tests.
    /// </summary>

    public interface IMLSTransactionScope
    {
        /// <summary>
        /// Commit this transaction to the DB.
        /// </summary>

        void Commit();
    }
}
