using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// Class that contains various options for filtering reports. This may expand over time, and/or may become a base class
    /// for report specific filter options.
    /// </summary>

    [DataContract]
    public class FilterReportOptions
    {
        /// <summary>
        /// True if orphaned (items that exist in the audits but not in the model. e.g. a CourseOffering that has been physically removed from the
        /// model by deleting its course from the tree) should be included.
        /// </summary>

        [DataMember]
        public bool IncludeOrphaned { get; set; }

        /// <summary>
        /// True if deleted items should be included.
        /// </summary>

        [DataMember]
        public bool IncludeDeleted { get; set; }
    }
}