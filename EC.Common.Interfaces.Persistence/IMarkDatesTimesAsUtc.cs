using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// When reading <c>DateTime</c> or <c>DateTime?</c> objects from EF they are marked as <c>Unspecified</c> even if 
    /// they were <c>Utc</c> when written to the database.
    /// All DateTimes must be converted to UTC before written to the Database to be consistent.
    /// This interface sets the <c>DateTime</c> and <c>DateTime?</c> properties of EF objects to <c>Utc</c> (<c>DateTime.Kind</c>) 
    /// when they are materialized by the db context.
    /// </summary>
    /// <remarks>
    /// This interface must be implemented by all models which contain properties of type <c>DateTime</c> or <c>DateTime?</c>.
    /// This is not a TimeZone conversion! It only marks the DateTimes as Utc.
    /// </remarks>

    public interface IMarkDateTimesAsUtc
    {

        /// <summary>
        /// Set all <c>DateTime</c> properties of a model to <c>Utc</c>.
        /// </summary>
        /// <example>
        /// If a model has two properties of type <c>DateTime</c>:
        /// 
        /// Person : IMarkDateTimeAsUtc
        /// {
        ///     string Name { get; set; }
        ///     DateTime JoinDate { get; set; }
        ///     DateTime? Birthday { get; set; } 
        /// 
        ///     public void MarkAllDateTimesAsUtc() 
        ///     {
        ///         if (this.JoinDate.Kind != DateTimeKind.Utc) {
        ///             this.JoinDate = DateTime.SpecifyKind(this.JoinDate, DateTimeKind.Utc);
        ///         }
        ///         
        ///         if ((this.Birthday.HasValue) && (this.Birthday.Value.Kind != DateTimeKind.Utc))
        ///         {
        ///             this.BirthDay = DateTime.SpecifyKind(this.BirthdayDate.Value, DateTimeKind.Utc);
        ///         }
        ///     }
        /// }
        /// 
        /// </example>

        void MarkAllDateTimesAsUtc();
    }
}
