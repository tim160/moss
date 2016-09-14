using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Order of sorting.
    /// <remarks>Used in GetQuestionPerformanceReport SPROC. You must fix the stored procedure if you change this.</remarks>
    /// </summary>

    [DataContract]
    public enum SortDirectionEnum
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Ascending = 1,

        [EnumMember]
        Descending = 2
    }

    /// <summary>
    /// Extension class to aid in dynamic linq sorting.
    /// </summary>

    public static class SortDirectionEnumExtensions
    {
        public static string ToAppendableSortString(this SortDirectionEnum sortDirection)
        {
            return sortDirection == SortDirectionEnum.Descending
                ? " DESC" 
                : " ASC";
        }

        public static string AppendAsSortStringTo(this SortDirectionEnum sortDirection, string str)
        {
            return str + ToAppendableSortString(sortDirection);
        }
    }
}
