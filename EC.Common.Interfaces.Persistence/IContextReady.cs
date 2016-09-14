using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// This interface is implemented by repositories to that they can obtain
    /// a DbContext to work with. It is used by the UnitofWork when it first
    /// created a repository of a given type.
    /// </summary>

    public interface IContextReady
    {
        /// <summary>
        /// Called when the data context is ready for the repository to use. Called
        /// but the MLSDBContext when it creates a new repository.
        /// </summary>
        /// <param name="context">DB context</param>

        void ContextReady(IMLSDbContext context);
    }
}
