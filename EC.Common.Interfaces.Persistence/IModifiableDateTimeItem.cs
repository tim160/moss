using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// This interface marks items with <c>ModifiedDate</c> and <c>CreatedDate</c>
    /// </summary>
    /// <remarks>
    /// This interface is used to update the <c>ModifiedDate</c> every time an item 
    /// is updated. The <c>ModifiedDate</c> is updated on commit with a commit handler.
    /// </remarks>

    public interface IModifiableDateTimeItem : IMarkDateTimesAsUtc
    {
        /// <summary>
        /// Date/time (in UTC) that item was last modified.
        /// </summary>

        DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Date/time (in UTC) that the answer was created.
        /// </summary>

        DateTime CreatedDate { get; }
    }
}
