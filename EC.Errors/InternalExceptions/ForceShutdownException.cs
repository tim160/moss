using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.InternalExceptions
{
    /// <summary>
    /// Thrown to signal that a shutdown is going on.
    /// </summary>

    public class ForceShutdownException : Exception
    {
        public ForceShutdownException() { }
    }
}
