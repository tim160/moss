using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EC.Constants;
using EC.Errors;
using EC.Common.Interfaces;

namespace EC.Service.DTO
{
    /// <summary>
    /// Simple class to hold information about how to sort a collection.
    /// </summary>

    [DataContract]
    public class SortOrder
    {
        [DataMember]
        public List<SortKey> SortKeys { get; set; }
    }

    /// <summary>
    /// Simple class to hold information about a single sort key for a collection.
    /// </summary>

    [DataContract]
    public class SortKey
    {
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public SortDirectionEnum SortDirection { get; set; }
    }

    /// <summary>
    /// Simple class to hold information for paging through a collection.
    /// </summary>

    [DataContract]
    public class PageInfo : IPageInfo
    {
        /// <summary>
        /// Starting index of the element (e.g. 0 for the first page).
        /// The start index is the number of elements to skip to show the next page.
        /// 
        /// Example: For a page size = 10: 
        /// -> start index for page 1 is 0.
        /// -> start index for page 2 is 10 (11st element in a zero-based list)
        /// -> start index for page 3 is 20 (21st element in a zero-based list)
        /// </summary>
        [DataMember] 
        public int StartIndex { get; set; }
        /// <summary>
        /// Maximal elements per page.
        /// </summary>
        [DataMember]
        public int? PageSize { get; set; }
    }

    /// <summary>
    /// Generic class for returning a list of items which is a subset of the total
    /// set of items available. Normally used when paging through a set of
    /// items.
    /// </summary>
    /// <typeparam name="T">type of the items in the subset</typeparam>

    [DataContract]
    public class CollectionSubset<T>
    {
        [DataMember]
        public List<T> Items { get; set; }

        /// <summary>
        /// Total count of result items.
        /// </summary>

        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public int StartIndex { get; set; }
    }

    /// <summary>
    /// Enumeration of the different NavPage types.
    /// Used to switch a NavPage between the different Views.
    /// </summary>

    [DataContract]
    public enum NavPageViewTypes
    {
        /// <summary>
        /// The Links widget
        /// </summary>
        [EnumMember]
        Links = 0,

        /// <summary>
        /// The TOC widget
        /// </summary>
        [EnumMember]
        TOC = 1
    }

    /// <summary>
    /// Enumeration with different link types (used for filters).
    /// To combine multiple types, use flags: Filter = LinkTypeFilterEnum.ExternalLinks | LinkTypeFilterEnum.NavPageLinks
    /// </summary>

    [DataContract]
    [Flags]
    public enum LinkTypeFilterEnum
    {
        /// <summary>
        /// Show all types of links/content items.
        /// </summary>

        [EnumMember]
        All = 1,

        /// <summary>
        /// Only external links.
        /// </summary>

        [EnumMember]
        ExternalLinks = 2,

        /// <summary>
        /// Only file links.
        /// </summary>

        [EnumMember]
        FileLinks = 4,

        /// <summary>
        /// Only nav page links.
        /// </summary>

        [EnumMember]
        NavPageLinks = 8
    }

    /// <summary>
    /// Filter for file collection content items (can be folders or simple content).
    /// </summary>

    [DataContract]
    [Flags]
    public enum FileCollectionContentTypeFilterEnum
    {
        /// <summary>
        /// All content items.
        /// </summary>

        [EnumMember]
        All = 1,

        /// <summary>
        /// Only folders.
        /// </summary>

        [EnumMember]
        Folders = 2,

        /// <summary>
        /// Only simple content (= files).
        /// </summary>

        [EnumMember]
        SimpleContent = 4
    }

    /// <summary>
    /// Filter for file collections/NavPages,...
    /// </summary>

    [DataContract]
    [Flags]
    public enum LocationFilterEnum
    {
        /// <summary>
        /// Return all file collections/pages (local, closest, all descendants, all shared items)
        /// </summary>

        [EnumMember]
        All = 1,

        /// <summary>
        /// Only return the local file collection/NavPage (if there is no local one - return nothing).
        /// </summary>

        [EnumMember]
        Local = 2,

        /// <summary>
        /// Return the closest file collection/NavPage (might be the local one or the first file collection further up the nav path).
        /// Note: In case of NavPages, this returns the parent of the current path.
        /// </summary>

        [EnumMember]
        Closest = 4,

        /// <summary>
        /// Descendant file collections/NavPages (local not included). Follow the path (links) further down to get all 
        /// descendants.
        /// </summary>

        [EnumMember]
        Decendants = 8,

        /// <summary>
        /// All ancestor file collections/NavPages (which are along the path).
        /// <example>
        /// Example for file collections: 
        /// If the path is '/Root/MarineLS/VMT' and Root and VMT have a file collection.
        /// All file collections along this path are returned.
        /// The file collection for Root and VMT are returned.
        /// This might be an overlapping for <c>Closest</c> and <c>Local</c>.
        /// </example>
        /// </summary>

        [EnumMember]
        Ancestors = 16,

        /// <summary>
        /// All shared file collections/NavPages. These items could be any of the above too - 
        /// dependent on whether the item has the shared flag enabled.
        /// </summary>

        [EnumMember]
        Shared = 32
    }
}