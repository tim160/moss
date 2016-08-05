using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace EC.Framework.Data
{
    public interface IGenDL
    {
        IDbCommand GetParameters(IDbCommand command, string name, object entity);
    }
}
