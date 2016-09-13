using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// This interface supports the injection of the IoC container into EF objects
    /// that are directly allocated by the infrastructure and hence cannot
    /// have the kernel injected via the IoC container.
    /// </summary>

    public interface IInjectKernel: IDisposable
    {
        /// <summary>
        /// Provides a reference to the IoC container to the callee.
        /// </summary>
        /// <param name="k">the IoC container</param>

        void InjectKernel(IKernel k);
    }
}
