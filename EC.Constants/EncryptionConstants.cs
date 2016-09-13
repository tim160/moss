using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    public static class EncryptionConstants
    {

        /// <summary>
        /// Workload for encryption with BCrypt.
        /// </summary>
        /// TODO: Set encryption workload to 13 (about 0.8 seconds per password).

        public const int ENCRYPTION_WORKLOAD = 5;
    }
}
