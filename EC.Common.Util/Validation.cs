using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; 

namespace EC.Common.Util
{
    public class Validation
    {
        public static bool IsEmailValid(string test)
        {
            Regex regex = new Regex("^[a-zA-Z0-9._-]+@([a-zA-Z0-9.-]+.)+[a-zA-Z0-9.-]{2,4}$");
            if (regex.IsMatch(test))
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

        public static IList<T> Intersect<T>(IList<T> lhs, IList<T> rhs)
        {
            if (lhs == null) throw new ArgumentNullException("lhs");
            if (rhs == null) throw new ArgumentNullException("rhs");

            List<T> result = new List<T>();

            // build the dictionary from the shorter list
            if (lhs.Count > rhs.Count)
            {
                IList<T> tmp = rhs;
                rhs = lhs;
                lhs = tmp;
            }
            Dictionary<T, bool> lookup = new Dictionary<T, bool>();
            foreach (T item in lhs)
            {
                if (!lookup.ContainsKey(item)) lookup.Add(item, true);
            }

            foreach (T item in rhs)
            {
                if (lookup.ContainsKey(item))
                {
                    lookup.Remove(item); // prevent duplicates
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
