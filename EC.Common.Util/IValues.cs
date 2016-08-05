using System;

namespace EC.Common.Util
{
    public interface IValues
    {
        /// <summary>
        /// a method prototype for getting the value of a field
        /// </summary>
        /// <param name="fieldName">the field name to be used for retrieving a value</param>
        /// <returns>the value for a given field name</returns>
        object GetValue(string name);

        /// <summary>
        /// a method prototype for setting the value of a field 
        /// </summary>
        /// <param name="fieldName">the field whose value is to be set</param>
        /// <param name="value">the value to be set to a field</param>
        void SetValue(string name, object value);

        /// <summary>
        /// a method prototype for determining if an object contains a field
        /// </summary>
        /// <param name="fieldName">The field to be searched</param>
        /// <returns>A boolean which indicates success or failure</returns>
        bool Contains(string name);
    }
}
