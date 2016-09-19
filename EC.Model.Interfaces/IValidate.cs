using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Model.Interfaces
{
    public interface IValidate
    {
        /// <summary>
        /// Check item's (context based) data whether it is in an consistent state to save.
        /// </summary>
        /// <param name="throwExceptionOnInvalidState">Optional: Flag whether to throw an exception instead of returning <c>false</c> in the case
        /// of an invalid state.</param>
        /// <returns>
        /// Return <c>true</c> if item is in an coherent state. Return <c>false</c> if item doesn't have all data to be saved or is in a corrupt state.
        /// </returns>

        bool Validate(bool throwExceptionOnInvalidState = false);
    }
}
