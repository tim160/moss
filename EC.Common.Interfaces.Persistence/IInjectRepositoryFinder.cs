using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// This interface supports the injection of a repository finder interface
    /// into EF objects that are directly allocated by the EF infrastructure
    /// and hence cannot use constructor injection.
    /// <remarks>
    /// <para>
    /// The RepositoryFinder cannot be resolved from the kernel (which the EF
    /// object should already have via IInjectKernel) because the Repository finder
    /// is scoped to the current UoW and it's not obvious how to get that
    /// scoping working correctly with Castle/Windsor.
    /// </para>
    /// <para>
    /// The RepositoryFinder should also be included as a constructor dependency
    /// in model objects. This constructor will only be used during unit tests, so
    /// for unit tests there will need to be a non-persistent implementation of
    /// the repository finder (and of the underlying repositories as well).
    /// </para>
    /// </remarks>
    /// </summary>

    public interface IInjectRepositoryFinder
    {
        /// <summary>
        /// Provide a repository finder to model classes.
        /// </summary>
        /// <param name="r">the repository finder</param>

        void InjectRepositoryFinder(ISimpleRepositoryFinder r);
    }
}
