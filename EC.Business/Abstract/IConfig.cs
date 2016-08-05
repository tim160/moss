using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Business.Abstract
{
    public interface IConfig
    {
        string GetConfig(string key);
        string GetConfig(string key, string defaultValue);
        int GetConfig(string key, int defaultValue);
        bool GetConfig(string key, bool defaultValue);
        decimal GetConfig(string key, decimal defaultValue);
    }
}
