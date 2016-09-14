using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Interfaces.Persistence
{

    /// <summary>
    /// Implement this interface on an item if the item could have dependent items like children 
    /// or optional other items with no cascade on delete action or Identifying Relationship. 
    /// They have then to be deleted manually. Otherwise they reside as orphans in the database.
    /// Manual deletion is needed for self-referencing tables like ContentItem.
    /// </summary>
    
    public interface ICascadeDelete
    {
        /// <summary>
        /// Return a list of all dependent items if this item is deleted (i.e. children, optional items).
        /// <remarks>
        /// The purpose of this call is to handle situations for which EF's normal cascading delete
        /// feature does not work. An example scenario in which this occurs is Object A has a required
        /// relationship to Object B, but Object B does not have a required relationship to Object A, but
        /// is dependent on Object A for its lifetime. This occurs, for example, with FileCollection and 
        /// Folder. A FileCollection must have a root folder, but since not all folders are root folders,
        /// there is no requirement that Folder have a required relationship to FileCollection. When
        /// a FileCollection object is deleted, EF will not cascade the delete to the root folder, but
        /// we do want the folder to be deleted. This call would then return the root folder, which
        /// would cause it to be deleted anyways (through some infrastructure code in BaseRepository).
        /// </remarks>
        /// </summary>
        /// <returns>Return a list of all dependent objects which must be deleted manually if this item is deleted.</returns>
       
        IList<object> GetDependentItems();
    }
}
