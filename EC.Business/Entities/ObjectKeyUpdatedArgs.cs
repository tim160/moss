using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Business
{
    public class ObjectKeyUpdatedArgs : EventArgs
    {
        public Guid ObjectKey { get; set; }
    }
}
