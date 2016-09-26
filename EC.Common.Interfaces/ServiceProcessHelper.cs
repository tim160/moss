using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public interface IServiceProcessHelper
    {
        /// <summary>
        /// Check if the passed in service name is started and stars it if it is not
        /// </summary>
        /// <param name="serviceName"></param>
        void CheckService(string serviceName);
        
    }
}
